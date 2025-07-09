using AutoMapper;
using BLL.DTO.TestResult;
using BLL.Interfaces;
using DAL.IRepository;
using DAL.Models;
using BLL.Utils;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class TestResultService : ITestResultService
{
    private readonly IUserRepository<User> _userRepository;
    private readonly IUserRepository<Patient> _patientRepository;
    private readonly IUserRepository<TestType> _testTypeRepository;
    private readonly IUserRepository<TestResult> _testResultRepository;
    private readonly IUserRepository<Doctor> _doctorRepository;
    private readonly IUserRepository<Appointment> _appointmentRepository;
    private readonly IMapper _mapper;
    private readonly IUserUtils _userUtils;
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    public TestResultService(IUserRepository<User> userRepository, IUserRepository<Patient> patientRepository, IUserRepository<TestType> testTypeRepository, IUserRepository<TestResult> testResultRepository, IUserRepository<Doctor> doctorRepository, IUserRepository<Appointment> appointmentRepository, IMapper mapper, IUserUtils userUtils, SWP391_RedRibbonLifeContext dbContext)
    {
        _userRepository = userRepository;
        _patientRepository = patientRepository;
        _testTypeRepository = testTypeRepository;
        _testResultRepository = testResultRepository;
        _doctorRepository = doctorRepository;
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
        _userUtils = userUtils;
        _dbContext = dbContext;
    }
    
    public async Task<TestResultDTO> CreateTestResultAsync(TestResultCreateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        await _userUtils.CheckPatientExistAsync(dto.PatientId);
        await _userUtils.CheckDoctorExistAsync(dto.DoctorId);
        await _userUtils.CheckDuplicateAppointmentAsync(dto.AppointmentId);
        var appointment = await _appointmentRepository.GetAsync(a => a.AppointmentId == dto.AppointmentId, true);
        if (appointment == null)
        {
            throw new Exception("Appointment not found.");
        }
        if (dto.TestTypeId != null)
        {
            await _userUtils.CheckTestTypeExistAsync(dto.TestTypeId.Value);
            if(appointment.TestTypeId != null && appointment.TestTypeId != dto.TestTypeId)
            {
                throw new Exception($"Appointment with ID {dto.AppointmentId} already has a different test type with ID {appointment.TestTypeId}.");
            }
        }
        else
        {
            if(appointment.TestTypeId == null)
            {
                throw new Exception($"Appointment with ID {dto.AppointmentId} does not have a test type.");
            }
        }
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            TestResult testResult = _mapper.Map<TestResult>(dto);
            if (dto.TestTypeId == null)
            {
                testResult.TestTypeId = appointment.TestTypeId.Value;
            }
            var createdTestResult = await _testResultRepository.CreateAsync(testResult);
            await transaction.CommitAsync();
            var fullTestResult = await _testResultRepository.GetWithRelationsAsync(
                t => t.TestResultId == createdTestResult.TestResultId, 
                useNoTracking: true,
                includeFunc: query => query.Include(t => t.TestType)
                              .Include(t => t.Patient).ThenInclude(p => p.User)
                              .Include(t => t.Doctor).ThenInclude(d => d.User)
                              .Include(t => t.Appointment)
            );
            return _mapper.Map<TestResultDTO>(fullTestResult);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<TestResultDTO> UpdateTestResultAsync(TestResultUpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        await dto.PatientId.ValidateIfNotNullAsync(_userUtils.CheckPatientExistAsync);
        await dto.DoctorId.ValidateIfNotNullAsync(_userUtils.CheckDoctorExistAsync);
        var appointment = await _appointmentRepository.GetAsync(a => a.AppointmentId == dto.AppointmentId, true);
        if (appointment == null)
        {
            throw new Exception("Appointment not found.");
        }
        if (dto.TestTypeId != null)
        {
            await _userUtils.CheckTestTypeExistAsync(dto.TestTypeId.Value);
            if(appointment.TestTypeId != null && appointment.TestTypeId != dto.TestTypeId)
            {
                throw new Exception($"Appointment with ID {dto.AppointmentId} already has a different test type with ID {appointment.TestTypeId}.");
            }
        }
        await dto.AppointmentId.ValidateIfNotNullAsync(_userUtils.CheckDuplicateAppointmentAsync);
        var testResult = await _testResultRepository.GetAsync(t => t.TestResultId == dto.TestResultId, true);
        if (testResult == null)
        {
            throw new Exception("Test result not found.");
        }
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            _mapper.Map(dto, testResult);
            var updatedTestResult = await _testResultRepository.UpdateAsync(testResult);
            await transaction.CommitAsync();
            var fullTestResult = await _testResultRepository.GetWithRelationsAsync(
                filter: t => t.TestResultId == updatedTestResult.TestResultId, 
                useNoTracking: true,
                includeFunc: query => query.Include(t => t.TestType)
                              .Include(t => t.Patient).ThenInclude(p => p.User)
                              .Include(t => t.Doctor).ThenInclude(d => d.User)
                              .Include(t => t.Appointment)
            );
            return _mapper.Map<TestResultDTO>(fullTestResult);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<List<TestResultDTO>> GetAllTestResultAsync()
    {
        var testResults = await _testResultRepository.GetAllWithRelationsAsync(
            includeFunc: query => query.Include(t => t.TestType)
                          .Include(t => t.Patient).ThenInclude(p => p.User)
                          .Include(t => t.Doctor).ThenInclude(d => d.User)
                          .Include(t => t.Appointment)
        );
        return _mapper.Map<List<TestResultDTO>>(testResults);
    }
    
    public async Task<TestResultDTO> GetTestResultByIdAsync(int id)
    {
        var testResult = await _testResultRepository.GetWithRelationsAsync(
            filter: t => t.TestResultId == id, 
            useNoTracking: true,
            includeFunc: query => query.Include(t => t.TestType)
                          .Include(t => t.Patient).ThenInclude(p => p.User)
                          .Include(t => t.Doctor).ThenInclude(d => d.User)
                          .Include(t => t.Appointment)
        );
        if (testResult == null)
        {
            throw new Exception("Test result not found.");
        }
        return _mapper.Map<TestResultDTO>(testResult);
    }
    
    public async Task<bool> DeleteTestResultByIdAsync(int id)
    {
        var testResult = await _testResultRepository.GetAsync(t => t.TestResultId == id, true);
        if (testResult == null)
        {
            throw new Exception("Test result not found.");
        }
        await _testResultRepository.DeleteAsync(testResult);
        return true;
    }
}