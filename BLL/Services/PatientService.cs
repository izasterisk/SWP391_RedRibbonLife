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
    private readonly IRepository<User> _repository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IUserUtils _userUtils;
    
    public PatientService(IMapper mapper, SWP391_RedRibbonLifeContext dbContext, IRepository<User> repository, IRepository<Patient> patientRepository, IUserUtils userUtils)
    {
        _mapper = mapper;
        _dbContext = dbContext;
        _repository = repository;
        _patientRepository = patientRepository;
        _userUtils = userUtils;
    }

    public async Task<PatientReadOnlyDTO> CreatePatientAsync(PatientCreateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        var usernameExists = await _repository.AnyAsync(u => u.Username.Equals(dto.Username));
        if (usernameExists)
        {
            throw new Exception($"Username {dto.Username} already exists.");
        }
        await _userUtils.CheckEmailExistAsync(dto.Email);
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            // Create User entity
            User user = _mapper.Map<User>(dto);
            user.IsActive = true;
            user.UserRole = "Patient";
            user.IsVerified = false;
            user.Password = _userUtils.CreatePasswordHash(dto.Password);
            // Save User first to get UserId
            var createdUser = await _repository.CreateAsync(user);
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
        var patient = await _patientRepository.GetAsync(p => p.PatientId == dto.PatientId, true);
        if (patient == null)
        {
            throw new Exception("Patient not found.");
        }
        var user = await _repository.GetAsync(u => u.UserId == patient.UserId, true);
        if (user == null)
        {
            throw new Exception($"User associated with patient ID {dto.PatientId} not found.");
        }
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            _mapper.Map(dto, user);
            _mapper.Map(dto, patient);
            await _repository.UpdateAsync(user);
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
        
        var user = await _repository.GetAsync(u => u.UserId == patient.UserId, true);
        if (user == null)
        {
            throw new Exception($"User associated with Patient ID {id} not found.");
        }
        
        await _patientRepository.DeleteAsync(patient);
        await _repository.DeleteAsync(user);
        return true;
    }
}