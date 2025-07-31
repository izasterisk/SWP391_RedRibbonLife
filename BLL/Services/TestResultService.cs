using AutoMapper;
using BLL.DTO.TestResult;
using BLL.Interfaces;
using DAL.IRepository;
using DAL.Models;
using BLL.Utils;

namespace BLL.Services;

public class TestResultService : ITestResultService
{
    private readonly IRepository<User> _repository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IRepository<TestType> _testTypeRepository;
    private readonly IRepository<Doctor> _doctorRepository;
    private readonly IRepository<Appointment> _appointmentRepository;
    private readonly ITestResultRepository _testResultRepository;
    private readonly IMapper _mapper;
    private readonly IUserUtils _userUtils;

    public TestResultService(IRepository<User> repository, IRepository<Patient> patientRepository, 
        IRepository<TestType> testTypeRepository, IRepository<Doctor> doctorRepository, 
        IRepository<Appointment> appointmentRepository, ITestResultRepository testResultRepository, 
        IMapper mapper, IUserUtils userUtils)
    {
        _repository = repository;
        _patientRepository = patientRepository;
        _testTypeRepository = testTypeRepository;
        _doctorRepository = doctorRepository;
        _appointmentRepository = appointmentRepository;
        _testResultRepository = testResultRepository;
        _mapper = mapper;
        _userUtils = userUtils;
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
        if (appointment.Status != "Confirmed")
        {
            throw new Exception("Appointment must be confirmed before creating test result.");
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

        TestResult testResult = _mapper.Map<TestResult>(dto);
        if (dto.TestTypeId == null)
        {
            testResult.TestTypeId = appointment.TestTypeId!.Value;
        }
        
        var createdTestResult = await _testResultRepository.CreateTestResultWithTransactionAsync(testResult, appointment);
        return _mapper.Map<TestResultDTO>(createdTestResult);
    }
    
    public async Task<TestResultDTO> UpdateTestResultAsync(TestResultUpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        if (dto.TestTypeId != null)
        {
            await _userUtils.CheckTestTypeExistAsync(dto.TestTypeId.Value);
        }
        
        var testResult = await _testResultRepository.GetTestResultForUpdateAsync(dto.TestResultId);
        if (testResult == null)
        {
            throw new Exception("Test result not found.");
        }
        
        _mapper.Map(dto, testResult);
        var updatedTestResult = await _testResultRepository.UpdateTestResultAsync(testResult);
        return _mapper.Map<TestResultDTO>(updatedTestResult);
    }
    
    public async Task<List<TestResultDTO>> GetAllTestResultAsync()
    {
        var testResults = await _testResultRepository.GetAllTestResultsWithRelationsAsync();
        return _mapper.Map<List<TestResultDTO>>(testResults);
    }
    
    public async Task<TestResultDTO> GetTestResultByIdAsync(int id)
    {
        var testResult = await _testResultRepository.GetTestResultWithRelationsAsync(id);
        if (testResult == null)
        {
            throw new Exception("Test result not found.");
        }
        return _mapper.Map<TestResultDTO>(testResult);
    }
    
    public async Task<List<TestResultDTO>> GetTestResultByDoctorIdAsync(int doctorId)
    {
        await _userUtils.CheckDoctorExistAsync(doctorId);
        var testResults = await _testResultRepository.GetTestResultsByDoctorIdWithRelationsAsync(doctorId);
        return _mapper.Map<List<TestResultDTO>>(testResults);
    }
    
    public async Task<bool> DeleteTestResultByIdAsync(int id)
    {
        var testResult = await _testResultRepository.GetTestResultForUpdateAsync(id);
        if (testResult == null)
        {
            throw new Exception("Test result not found.");
        }
        return await _testResultRepository.DeleteTestResultAsync(testResult);
    }
}