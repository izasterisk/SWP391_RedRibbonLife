using AutoMapper;
using BLL.DTO.Patient;
using BLL.Interfaces;
using BLL.Utils;
using DAL.IRepository;
using DAL.Models;

namespace BLL.Services;

public class PatientService : IPatientService
{
    private readonly IMapper _mapper;
    private readonly IPatientRepository _patientRepository;
    private readonly IUserUtils _userUtils;
    
    public PatientService(IMapper mapper, IPatientRepository patientRepository, IUserUtils userUtils)
    {
        _mapper = mapper;
        _patientRepository = patientRepository;
        _userUtils = userUtils;
    }

    public async Task<PatientReadOnlyDTO> CreatePatientAsync(PatientCreateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        
        if (string.IsNullOrEmpty(dto.Username))
        {
            throw new ArgumentException("Username cannot be null or empty.");
        }
        if (string.IsNullOrEmpty(dto.Email))
        {
            throw new ArgumentException("Email cannot be null or empty.");
        }
        
        var usernameExists = await _patientRepository.CheckUsernameExistsAsync(dto.Username);
        if (usernameExists)
        {
            throw new Exception($"Username {dto.Username} already exists.");
        }
        await _userUtils.CheckEmailExistAsync(dto.Email);
        
        // Create User entity
        User user = _mapper.Map<User>(dto);
        user.IsActive = true;
        user.UserRole = "Patient";
        user.IsVerified = false;
        user.Password = _userUtils.CreatePasswordHash(dto.Password);
        
        // Create Patient entity
        Patient patient = _mapper.Map<Patient>(dto);
        
        var createdPatient = await _patientRepository.CreatePatientWithTransactionAsync(user, patient);
        var detailedPatient = await _patientRepository.GetPatientWithUserAsync(createdPatient.PatientId, true);
        
        if (detailedPatient == null)
        {
            throw new Exception("Patient after created not found.");
        }
        return _mapper.Map<PatientReadOnlyDTO>(detailedPatient);
    }
    
    public async Task<PatientReadOnlyDTO> UpdatePatientAsync(PatientUpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        
        var patient = await _patientRepository.GetPatientForUpdateAsync(dto.PatientId);
        if (patient == null)
        {
            throw new Exception("Patient not found.");
        }
        var user = await _patientRepository.GetUserByUserIdAsync(patient.UserId, false);
        if (user == null)
        {
            throw new Exception($"User associated with patient ID {dto.PatientId} not found.");
        }
        
        _mapper.Map(dto, user);
        _mapper.Map(dto, patient);
        
        var updatedPatient = await _patientRepository.UpdatePatientWithTransactionAsync(user, patient);
        var detailedPatient = await _patientRepository.GetPatientWithUserAsync(updatedPatient.PatientId, true);
        
        if (detailedPatient == null)
        {
            throw new Exception("Patient after updated not found.");
        }
        return _mapper.Map<PatientReadOnlyDTO>(detailedPatient);
    }

    public async Task<List<PatientReadOnlyDTO>> GetAllActivePatientsAsync()
    {
        var patients = await _patientRepository.GetAllActivePatientsWithUserAsync();
        return _mapper.Map<List<PatientReadOnlyDTO>>(patients);
    }

    public async Task<PatientReadOnlyDTO> GetPatientByPatientIDAsync(int id)
    {
        var patient = await _patientRepository.GetPatientWithUserAsync(id, true);
        if (patient == null)
        {
            throw new Exception("Patient not found.");
        }
        return _mapper.Map<PatientReadOnlyDTO>(patient);
    }
    
    public async Task<bool> DeletePatientAsync(int id)
    {
        var patient = await _patientRepository.GetPatientForUpdateAsync(id);
        if (patient == null)
        {
            throw new Exception("Patient not found.");
        }
        
        var user = await _patientRepository.GetUserByUserIdAsync(patient.UserId, false);
        if (user == null)
        {
            throw new Exception($"User associated with Patient ID {id} not found.");
        }
        
        return await _patientRepository.DeletePatientWithTransactionAsync(patient, user);
    }
}