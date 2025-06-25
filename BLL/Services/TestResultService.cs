using AutoMapper;
using BLL.DTO.TestResult;
using BLL.Interfaces;
using DAL.IRepository;
using DAL.Models;
using BLL.Utils;

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
        _userUtils.CheckPatientExist(dto.PatientId);
        _userUtils.CheckDoctorExist(dto.DoctorId);
        _userUtils.CheckTestTypeExist(dto.TestTypeId);
        dto.AppointmentId.ValidateIfNotNull(_userUtils.CheckDuplicateAppointment);
        
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            TestResult testResult = _mapper.Map<TestResult>(dto);
            var createdTestResult = await _testResultRepository.CreateAsync(testResult);
            await transaction.CommitAsync();
            var fullTestResult = await _testResultRepository.GetWithRelationsAsync(
                t => t.TestResultId == createdTestResult.TestResultId, 
                true,
                t => t.TestType,
                t => t.Patient.User,
                t => t.Doctor.User,
                t => t.Appointment
            );
            return _mapper.Map<TestResultDTO>(fullTestResult);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<dynamic> UpdateTestResultAsync(TestResultUpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        dto.PatientId.ValidateIfNotNull(_userUtils.CheckPatientExist);
        dto.DoctorId.ValidateIfNotNull(_userUtils.CheckDoctorExist);
        dto.TestTypeId.ValidateIfNotNull(_userUtils.CheckTestTypeExist);
        dto.AppointmentId.ValidateIfNotNull(_userUtils.CheckDuplicateAppointment);
        // Get existing test result
        var testResult = await _testResultRepository.GetAsync(t => t.TestResultId == dto.TestResultId, true);
        if (testResult == null)
        {
            throw new Exception("Test result not found.");
        }
        // Update test result
        _mapper.Map(dto, testResult);
        var fullTestResult = await _testResultRepository.GetWithRelationsAsync(
            t => t.TestResultId == testResult.TestResultId, 
            true,
            t => t.TestType,
            t => t.Patient.User,
            t => t.Doctor.User,
            t => t.Appointment
        );
        return _mapper.Map<TestResultDTO>(fullTestResult);
    }
    
    public async Task<List<TestResultDTO>> GetAllTestResultAsync()
    {
        var testResults = await _testResultRepository.GetAllWithRelationsAsync(
            t => t.TestType,
            t => t.Patient.User,
            t => t.Doctor.User,
            t => t.Appointment
        );
        return _mapper.Map<List<TestResultDTO>>(testResults);
    }
    
    public async Task<TestResultDTO> GetTestResultByIdAsync(int id)
    {
        var testResult = await _testResultRepository.GetAsync(t => t.TestResultId == id, true);
        if (testResult == null)
        {
            throw new Exception("Test result not found.");
        }
        var fullTestResult = await _testResultRepository.GetWithRelationsAsync(
            t => t.TestResultId == testResult.TestResultId, 
            true,
            t => t.TestType,
            t => t.Patient.User,
            t => t.Doctor.User,
            t => t.Appointment
        );
        return _mapper.Map<TestResultDTO>(fullTestResult);
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