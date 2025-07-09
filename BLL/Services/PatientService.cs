using AutoMapper;
using BLL.DTO.Patient;
using BLL.Interfaces;
using BLL.Utils;
using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class PatientService : IPatientService
{
    private readonly IMapper _mapper;
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    private readonly IUserRepository<User> _userRepository;
    private readonly IUserRepository<Patient> _patientRepository;
    private readonly IUserUtils _userUtils;
    
    public PatientService(IMapper mapper, SWP391_RedRibbonLifeContext dbContext, IUserRepository<User> userRepository, IUserRepository<Patient> patientRepository, IUserUtils userUtils)
    {
        _mapper = mapper;
        _dbContext = dbContext;
        _userRepository = userRepository;
        _patientRepository = patientRepository;
        _userUtils = userUtils;
    }

    public async Task<PatientReadOnlyDTO> CreatePatientAsync(PatientCreateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        // Check if username already exists
        var existingUser = await _userRepository.GetAsync(u => u.Username.Equals(dto.Username));
        if (existingUser != null)
        {
            throw new Exception($"Username {dto.Username} already exists.");
        }
        // Check if email already exists
        var existingUserByEmail = await _userRepository.GetAsync(u => u.Email.Equals(dto.Email));
        if (existingUserByEmail != null)
        {
            throw new Exception($"Email {dto.Email} already exists.");
        }
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            // Create User entity
            User user = _mapper.Map<User>(dto);
            user.IsActive = true;
            user.UserRole = "Patient";
            user.IsVerified = false;
            user.Password = _userUtils.CreatePasswordHash(dto.Password);
            
            // Save User first to get UserId
            var createdUser = await _userRepository.CreateAsync(user);
            
            // Create Patient entity
            Patient patient = _mapper.Map<Patient>(dto);
            patient.UserId = createdUser.UserId;
            
            var createdPatient = await _patientRepository.CreateAsync(patient);
            await transaction.CommitAsync();
            
            var detailedPatient = await _patientRepository.GetWithRelationsAsync(
                filter: p => p.PatientId == createdPatient.PatientId,
                useNoTracking: true,
                includeFunc: query => query.Include(p => p.User)
            );
            if (detailedPatient == null)
            {
                throw new Exception("Patient after created not found.");
            }
            return _mapper.Map<PatientReadOnlyDTO>(detailedPatient);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<PatientReadOnlyDTO> UpdatePatientAsync(PatientUpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var patient = await _patientRepository.GetAsync(p => p.PatientId == dto.PatientId, true);
            if (patient == null)
            {
                throw new Exception("Patient not found.");
            }
            var user = await _userRepository.GetAsync(u => u.UserId == patient.UserId, true);
            if (user == null)
            {
                throw new Exception($"User associated with patient ID {dto.PatientId} not found.");
            }
            _mapper.Map(dto, user);
            _mapper.Map(dto, patient);
            await _userRepository.UpdateAsync(user);
            var updatedPatient = await _patientRepository.UpdateAsync(patient);
            await transaction.CommitAsync();
            var detailedPatient = await _patientRepository.GetWithRelationsAsync(
                filter: p => p.PatientId == updatedPatient.PatientId,
                useNoTracking: true,
                includeFunc: query => query.Include(p => p.User)
            );
            if (detailedPatient == null)
            {
                throw new Exception("Patient after updated not found.");
            }
            return _mapper.Map<PatientReadOnlyDTO>(detailedPatient);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<PatientReadOnlyDTO>> GetAllActivePatientsAsync()
    {
        var patients = await _patientRepository.GetAllWithRelationsAsync(
            includeFunc: query => query.Include(p => p.User)
                .Where(p => p.User.IsActive && p.User.UserRole == "Patient")
        );
        return _mapper.Map<List<PatientReadOnlyDTO>>(patients);
    }

    public async Task<PatientReadOnlyDTO> GetPatientByPatientIDAsync(int id)
    {
        var patient = await _patientRepository.GetWithRelationsAsync(
            filter: p => p.PatientId == id,
            useNoTracking: true,
            includeFunc: query => query.Include(p => p.User)
        );
        if (patient == null)
        {
            throw new Exception("Patient not found.");
        }
        return _mapper.Map<PatientReadOnlyDTO>(patient);
    }
    
    public async Task<bool> DeletePatientAsync(int id)
    {
        var patient = await _patientRepository.GetAsync(p => p.PatientId == id, true);
        if (patient == null)
        {
            throw new Exception("Patient not found.");
        }
        
        var user = await _userRepository.GetAsync(u => u.UserId == patient.UserId, true);
        if (user == null)
        {
            throw new Exception($"User associated with Patient ID {id} not found.");
        }
        
        await _patientRepository.DeleteAsync(patient);
        await _userRepository.DeleteAsync(user);
        return true;
    }
}