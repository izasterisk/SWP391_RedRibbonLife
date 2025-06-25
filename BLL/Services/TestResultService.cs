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
    private readonly IMapper _mapper;
    private readonly IUserUtils _userUtils;
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    public TestResultService(IUserRepository<User> userRepository, IUserRepository<Patient> patientRepository, IUserRepository<TestType> testTypeRepository, IUserRepository<TestResult> testResultRepository, IMapper mapper, IUserUtils userUtils, SWP391_RedRibbonLifeContext dbContext)
    {
        _userRepository = userRepository;
        _patientRepository = patientRepository;
        _testTypeRepository = testTypeRepository;
        _testResultRepository = testResultRepository;
        _mapper = mapper;
        _userUtils = userUtils;
        _dbContext = dbContext;
    }
    
    public async Task<dynamic> CreateTestResultAsync(TestResultCreateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        _userUtils.CheckPatientExist(dto.PatientId);
        _userUtils.CheckDoctorExist(dto.DoctorId);
        _userUtils.CheckTestTypeExist(dto.TestTypeId);
        dto.AppointmentId.ValidateIfNotNull(_userUtils.CheckAppointmentExist);
        // Begin transaction
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            // Create test result
            TestResult testResult = _mapper.Map<TestResult>(dto);
            var createdTestResult = await _testResultRepository.CreateAsync(testResult);
            // Commit transaction
            await transaction.CommitAsync();
            return new
            {
                TestResultInfo = _mapper.Map<TestResultDTO>(createdTestResult)
            };
        }
        catch (Exception)
        {
            // Rollback transaction on error
            await transaction.RollbackAsync();
            throw; // Re-throw the exception
        }
    }
    
    public async Task<dynamic> UpdateTestResultAsync(TestResultUpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        dto.PatientId.ValidateIfNotNull(_userUtils.CheckPatientExist);
        dto.DoctorId.ValidateIfNotNull(_userUtils.CheckDoctorExist);
        dto.TestTypeId.ValidateIfNotNull(_userUtils.CheckTestTypeExist);
        dto.AppointmentId.ValidateIfNotNull(_userUtils.CheckAppointmentExist);
        
        // Get existing test result
        var testResult = await _testResultRepository.GetAsync(t => t.TestResultId == dto.TestResultId, true);
        if (testResult == null)
        {
            throw new Exception("Test result not found.");
        }
        // Update test result
        _mapper.Map(dto, testResult);
        var updatedTestResult = await _testResultRepository.UpdateAsync(testResult);
        return new
        {
            TestResultInfo = _mapper.Map<TestResultDTO>(updatedTestResult)
        };
    }
}