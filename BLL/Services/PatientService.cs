using AutoMapper;
using BLL.DTO.Patient;
using BLL.DTO.User;
using BLL.Interfaces;
using DAL.IRepository;
using DAL.Models;

namespace BLL.Services;

public class PatientService : IPatientService
{
    private readonly IUserRepository<User> _userRepository;
    private readonly IUserRepository<Patient> _patientRepository;
    private readonly IMapper _mapper;
    private readonly IUserUtils _userUtils;
    public PatientService(IUserRepository<User> userRepository, IUserRepository<Patient> patientRepository, IMapper mapper, IUserUtils userUtils)
    {
        _userRepository = userRepository;
        _patientRepository = patientRepository;
        _mapper = mapper;
        _userUtils = userUtils;
    }

    public async Task<dynamic> CreatePatientAsync(PatientCreateDTO dto)
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
        // Create User entity
        User user = _mapper.Map<User>(dto);
        user.IsActive = true; // Set default value for IsActive
        user.UserRole = "Patient"; // Set default value for UserRole
        user.IsVerified = false; // Default
        user.Password = _userUtils.CreatePasswordHash(dto.Password);
        // Save User first to get UserId
        var createdUser = await _userRepository.CreateAsync(user);
        // Create Patient entity with UserId from created User
        Patient patient = new Patient
        {
            UserId = createdUser.UserId,
            BloodType = dto.BloodType,
            IsPregnant = dto.IsPregnant,
            SpecialNotes = dto.SpecialNotes
        };
        // Save
        var createdPatient = await _patientRepository.CreateAsync(patient);
        return new PatientReadOnlyDTO
        {
            // User properties
            Username = createdUser.Username,
            Email = createdUser.Email,
            PhoneNumber = createdUser.PhoneNumber,
            FullName = createdUser.FullName,
            DateOfBirth = createdUser.DateOfBirth,
            Gender = createdUser.Gender,
            Address = createdUser.Address,
            // Patient properties
            PatientId = createdPatient.PatientId,
            BloodType = createdPatient.BloodType,
            IsPregnant = createdPatient.IsPregnant,
            SpecialNotes = createdPatient.SpecialNotes
        };
    }
    public async Task<dynamic> UpdatePatientAsync(PatientUpdateDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
            // Get patient first
            var patient = await _patientRepository.GetAsync(d => d.PatientId == dto.PatientId);
            if (patient == null)
            {
                throw new Exception("Patient not found.");
            }
            var user = await _userRepository.GetAsync(u => u.UserId == patient.UserId);
            if (user == null)
            {
                throw new Exception($"User associated with patient ID {dto.PatientId} not found.");
            }
            _mapper.Map(dto, user);
            _mapper.Map(dto, patient);
            // Save changes
            var updatedUser = await _userRepository.UpdateAsync(user);
            var updatedPatient = await _patientRepository.UpdateAsync(patient);
            return new PatientReadOnlyDTO
            {
                // User properties
                Username = updatedUser.Username,
                Email = updatedUser.Email,
                PhoneNumber = updatedUser.PhoneNumber,
                FullName = updatedUser.FullName,
                DateOfBirth = updatedUser.DateOfBirth,
                Gender = updatedUser.Gender,
                Address = updatedUser.Address,
                // Patient properties
                PatientId = updatedPatient.PatientId,
                BloodType = updatedPatient.BloodType,
                IsPregnant = updatedPatient.IsPregnant,
                SpecialNotes = updatedPatient.SpecialNotes
            };
        }

    public async Task<List<PatientReadOnlyDTO>> GetAllActivePatientsAsync()
    {
        // Get all patients with their associated users
        var users = await _userRepository.GetAllByFilterAsync(d => d.IsActive && d.UserRole == "Patient", true);
        if (users == null || !users.Any())
        {
            throw new Exception("No active patients found.");
        }
        var patientReadOnlyDTOs = new List<PatientReadOnlyDTO>();
        foreach (var user in users)
        {
            // Get the associated patient
            var patient = await _patientRepository.GetAsync(u => u.UserId == user.UserId, true);
            if (patient != null)
            {
                // Create a combined DTO manually to ensure proper mapping
                var patientReadOnlyDTO = new PatientReadOnlyDTO
                {
                    // User properties
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    FullName = user.FullName,
                    DateOfBirth = user.DateOfBirth,
                    Gender = user.Gender,
                    Address = user.Address,
                    // Patient properties
                    PatientId = patient.PatientId,
                    BloodType = patient.BloodType,
                    IsPregnant = patient.IsPregnant,
                    SpecialNotes = patient.SpecialNotes
                };
                patientReadOnlyDTOs.Add(patientReadOnlyDTO);
            }
        }
        return patientReadOnlyDTOs;
    }

    public async Task<PatientReadOnlyDTO> GetPatientByPatientIDAsync(int id)
    {
        // Get patient by ID
        var patient = await _patientRepository.GetAsync(u => u.PatientId == id);
        if (patient == null)
        {
            throw new Exception($"Patient with ID {id} not found.");
        }
        // Get the associated user
        var user = await _userRepository.GetAsync(u => u.UserId == patient.UserId);
        if (user == null)
        {
            throw new Exception($"User associated with Patient ID {id} not found.");
        }
        var patientReadOnlyDTO = new PatientReadOnlyDTO
        {
            // User properties
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            FullName = user.FullName,
            DateOfBirth = user.DateOfBirth,
            Gender = user.Gender,
            Address = user.Address,
            // Patient properties
            PatientId = patient.PatientId,
            BloodType = patient.BloodType,
            IsPregnant = patient.IsPregnant,
            SpecialNotes = patient.SpecialNotes
        };
        return patientReadOnlyDTO;
    }
    public async Task<bool> DeletePatientAsync(int id)
    {
        // Get patient by ID
        var patient = await _patientRepository.GetAsync(u => u.PatientId == id);
        if (patient == null)
        {
            throw new Exception($"Patient with ID {id} not found.");
        }
        // Get the associated user
        var user = await _userRepository.GetAsync(u => u.UserId == patient.UserId);
        if (user == null)
        {
            throw new Exception($"User associated with Patient ID {id} not found.");
        }
        // Delete user
        await _patientRepository.DeleteAsync(patient);
        await _userRepository.DeleteAsync(user);
        return true;
    }
}