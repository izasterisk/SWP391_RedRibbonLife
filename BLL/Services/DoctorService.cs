using AutoMapper;
using BLL.DTO.Doctor;
using BLL.Interfaces;
using DAL.IRepository;
using DAL.Models;

namespace BLL.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IUserRepository<User> _userRepository;
        private readonly IUserRepository<Doctor> _doctorRepository;
        private readonly IMapper _mapper;
        private readonly IUserUtils _userUtils;

        public DoctorService(IUserRepository<User> userRepository, IUserRepository<Doctor> doctorRepository, IMapper mapper, IUserUtils userUtils)
        {
            _userRepository = userRepository;
            _doctorRepository = doctorRepository;
            _mapper = mapper;
            _userUtils = userUtils;
        }

        public async Task<dynamic> CreateDoctorAsync(DoctorCreateDTO dto)
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
            user.UserRole = "Doctor"; // Set default value for UserRole
            user.IsVerified = false; // Default
            user.Password = _userUtils.CreatePasswordHash(dto.Password);
            // Save User first to get UserId
            var createdUser = await _userRepository.CreateAsync(user);
            // Create Doctor entity with UserId from created User
            Doctor doctor = new Doctor
            {
                UserId = createdUser.UserId,
                DoctorImage = dto.DoctorImage,
                Bio = dto.Bio
            };
            // Save
            var createdDoctor = await _doctorRepository.CreateAsync(doctor);
            return new DoctorReadOnlyDTO
            {
                // User properties
                Username = createdUser.Username,
                Email = createdUser.Email,
                PhoneNumber = createdUser.PhoneNumber,
                FullName = createdUser.FullName,
                DateOfBirth = createdUser.DateOfBirth,
                Gender = createdUser.Gender,
                Address = createdUser.Address,
                // Doctor properties
                DoctorId = createdDoctor.DoctorId,
                DoctorImage = createdDoctor.DoctorImage,
                Bio = createdDoctor.Bio
            };
        }
        
        public async Task<dynamic> UpdateDoctorAsync(DoctorUpdateDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
            // Get existing doctor
            var doctor = await _doctorRepository.GetAsync(d => d.DoctorId == dto.DoctorId);
            if (doctor == null)
            {
                throw new Exception("Doctor not found.");
            }
            var user = await _userRepository.GetAsync(u => u.UserId == doctor.UserId);
            if (user == null)
            {
                throw new Exception($"User associated with doctor ID {dto.DoctorId} not found.");
            }
            _mapper.Map(dto, user);
            _mapper.Map(dto, doctor);
            // Save changes
            var updatedUser = await _userRepository.UpdateAsync(user);
            var updatedDoctor = await _doctorRepository.UpdateAsync(doctor);
            return new DoctorReadOnlyDTO
            {
                // User properties
                Username = updatedUser.Username,
                Email = updatedUser.Email,
                PhoneNumber = updatedUser.PhoneNumber,
                FullName = updatedUser.FullName,
                DateOfBirth = updatedUser.DateOfBirth,
                Gender = updatedUser.Gender,
                Address = updatedUser.Address,
                // Doctor properties
                DoctorId = updatedDoctor.DoctorId,
                DoctorImage = updatedDoctor.DoctorImage,
                Bio = updatedDoctor.Bio
            };
        }
        public async Task<List<DoctorReadOnlyDTO>> GetAllDoctorsAsync()
        {
            // Get all doctors with their associated users
            var users = await _userRepository.GetAllByFilterAsync(d => d.IsActive && d.UserRole == "Doctor", true);
            if (users == null || !users.Any())
            {
                throw new Exception("No active doctors found.");
            }
            var doctorReadOnlyDTOs = new List<DoctorReadOnlyDTO>();
            foreach (var user in users)
            {
                // Get the associated user
                var doctor = await _doctorRepository.GetAsync(u => u.UserId == user.UserId, true);
                if (doctor != null)
                {
                    // Create a combined DTO manually to ensure proper mapping
                    var doctorReadOnlyDTO = new DoctorReadOnlyDTO
                    {
                        // User properties (excluding password)
                        // UserId = user.UserId,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        FullName = user.FullName,
                        DateOfBirth = user.DateOfBirth,
                        Gender = user.Gender,
                        Address = user.Address,
                        // UserRole = user.UserRole,
                        // IsActive = user.IsActive,
                        // IsVerified = user.IsVerified,

                        // Doctor properties
                        DoctorId = doctor.DoctorId,
                        DoctorImage = doctor.DoctorImage,
                        Bio = doctor.Bio
                    };
                    doctorReadOnlyDTOs.Add(doctorReadOnlyDTO);
                }
            }
            return doctorReadOnlyDTOs;
        }
        public async Task<DoctorReadOnlyDTO> GetDoctorByDoctorIDAsync(int id)
        {
            // Get doctor by ID
            var doctor = await _doctorRepository.GetAsync(u => u.DoctorId == id);
            if (doctor == null)
            {
                throw new Exception($"Doctor with ID {id} not found.");
            }
            // Get the associated user
            var user = await _userRepository.GetAsync(u => u.UserId == doctor.UserId);
            if (user == null)
            {
                throw new Exception($"User associated with Doctor ID {id} not found.");
            }
            // Create a combined DTO manually to ensure proper mapping
            var doctorReadOnlyDTO = new DoctorReadOnlyDTO
            {
                // User properties
                //UserId = user.UserId,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FullName = user.FullName,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                Address = user.Address,
                // UserRole = user.UserRole,
                // IsActive = user.IsActive,
                // IsVerified = user.IsVerified,

                // Doctor properties
                DoctorId = doctor.DoctorId,
                DoctorImage = doctor.DoctorImage,
                Bio = doctor.Bio
            };
            return doctorReadOnlyDTO;
        }
        
        public async Task<bool> DeleteDoctorByDoctorIdAsync(int id)
        {
            // Get doctor by ID
            var doctor = await _doctorRepository.GetAsync(u => u.DoctorId == id);
            if (doctor == null)
            {
                throw new Exception($"Doctor with ID {id} not found.");
            }
            // Get the associated user
            var user = await _userRepository.GetAsync(u => u.UserId == doctor.UserId);
            if (user == null)
            {
                throw new Exception($"User associated with Doctor ID {id} not found.");
            }
            // Delete user
            await _doctorRepository.DeleteAsync(doctor);
            await _userRepository.DeleteAsync(user);
            return true;
        }
    }
}
