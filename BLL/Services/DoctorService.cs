using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BLL.DTO.Doctor;
using BLL.DTO.User;
using BLL.Interfaces;
using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

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

        public int GetDoctorIdByUserId(int id)
        {
            var doctor = _doctorRepository.GetAsync(d => d.UserId == id).Result;
            if (doctor == null)
            {
                throw new Exception($"Doctor with {id} not found!!!.");
            }
            return doctor.DoctorId;
        }

        public async Task<bool> CreateDoctorAsync(DoctorDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");

            // Validate required fields
            if (string.IsNullOrWhiteSpace(dto.Username))
                throw new ArgumentNullException(nameof(dto.Username), "Username is required.");
            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ArgumentNullException(nameof(dto.Password), "Password is required.");
            if (string.IsNullOrWhiteSpace(dto.FullName))
                throw new ArgumentNullException(nameof(dto.FullName), "Full name is required.");
            if (string.IsNullOrWhiteSpace(dto.Gender))
                throw new ArgumentNullException(nameof(dto.Gender), "Gender is required.");
            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
                throw new ArgumentNullException(nameof(dto.PhoneNumber), "Phone number is required.");

            // Check if username already exists
            var existingUser = await _userRepository.GetAsync(u => u.Username.Equals(dto.Username));
            if (existingUser != null)
            {
                throw new Exception($"Username {dto.Username} already exists.");
            }

            // Check if email already exists (only if email is provided)
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var existingUserByEmail = await _userRepository.GetAsync(u => u.Email.Equals(dto.Email));
                if (existingUserByEmail != null)
                {
                    throw new Exception($"Email {dto.Email} already exists.");
                }
            }

            // Create User entity
            User user = _mapper.Map<User>(dto);
            user.IsActive = true; // Set default value for IsActive
            user.UserRole = "Doctor"; // Set default value for UserRole
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

            // Save Doctor
            await _doctorRepository.CreateAsync(doctor);

            return true;
        }

        public async Task<bool> UpdateDoctorAsync(DoctorDTO dto, ClaimsPrincipal userClaims)
        {
            ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
            ArgumentNullException.ThrowIfNull(userClaims, $"{nameof(userClaims)} is null");

            // Lấy thông tin từ token claims
            var currentUserRole = userClaims.FindFirst(ClaimTypes.Role)?.Value;
            var currentUserIdStr = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserRole))
            {
                throw new UnauthorizedAccessException("User role not found in token claims.");
            }

            if (string.IsNullOrEmpty(currentUserIdStr) || !int.TryParse(currentUserIdStr, out int currentUserId))
            {
                throw new UnauthorizedAccessException("User ID not found or invalid in token claims.");
            }

            // Nếu role là "Doctor", tự động sử dụng userId từ token claims
            if (currentUserRole.Equals("Doctor", StringComparison.OrdinalIgnoreCase))
            {
                dto.UserId = currentUserId;
            }
            // Nếu role khác "Doctor" (Admin, Manager), giữ nguyên userId từ DTO
            else
            {
                // Validate that non-Doctor roles provide a valid UserId
                if (dto.UserId <= 0)
                {
                    throw new ArgumentException("User ID must be provided and greater than 0 for Admin/Manager roles.");
                }
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(dto.Username))
                throw new ArgumentNullException(nameof(dto.Username), "Username is required.");
            if (string.IsNullOrWhiteSpace(dto.FullName))
                throw new ArgumentNullException(nameof(dto.FullName), "Full name is required.");

            // Get user first
            var user = await _userRepository.GetAsync(u => u.UserId == dto.UserId);
            if (user == null)
            {
                throw new Exception($"User associated with Doctor ID {dto.UserId} not found.");
            }

            // Get existing doctor
            var doctor = await _doctorRepository.GetAsync(d => d.DoctorId == GetDoctorIdByUserId(dto.UserId));
            if (doctor == null)
            {
                throw new Exception($"Doctor not found.");
            }

            // Check if username already exists (excluding current user)
            var userWithSameUsername = await _userRepository.GetAsync(u => u.Username.Equals(dto.Username) && u.UserId != user.UserId);
            if (userWithSameUsername != null)
            {
                throw new Exception($"Username {dto.Username} already exists.");
            }

            // Check if email already exists (only if email is provided and excluding current user)
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var userWithSameEmail = await _userRepository.GetAsync(u => u.Email.Equals(dto.Email) && u.UserId != user.UserId);
                if (userWithSameEmail != null)
                {
                    throw new Exception($"Email {dto.Email} already exists.");
                }
            }

            // Update User entity
            user.Username = dto.Username;
            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;
            user.FullName = dto.FullName;
            user.DateOfBirth = dto.DateOfBirth;
            user.Gender = dto.Gender;
            user.Address = dto.Address;

            // Update Doctor entity
            doctor.DoctorImage = dto.DoctorImage;
            doctor.Bio = dto.Bio;

            // Save changes
            await _userRepository.UpdateAsync(user);
            await _doctorRepository.UpdateAsync(doctor);
            return true;
        }

        public async Task<List<DoctorReadOnlyDTO>> GetAllDoctorsAsync()
        {
            // Get all doctors with their associated users
            var users = await _userRepository.GetAllByFilterAsync(d => d.IsActive, true);
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
                        UserId = user.UserId,
                        Username = user.Username,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        FullName = user.FullName,
                        DateOfBirth = user.DateOfBirth,
                        Gender = user.Gender,
                        Address = user.Address,
                        UserRole = user.UserRole,
                        IsActive = user.IsActive,

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
        public async Task<List<DoctorReadOnlyDTO>> GetDoctorByFullnameAsync(string fullname)
        {
            // Get all doctors with their associated users
            var users = await _userRepository.GetAllByFilterAsync(u => u.IsActive && u.FullName.Equals(fullname), true);
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
                        UserId = user.UserId,
                        Username = user.Username,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        FullName = user.FullName,
                        DateOfBirth = user.DateOfBirth,
                        Gender = user.Gender,
                        Address = user.Address,
                        UserRole = user.UserRole,
                        IsActive = user.IsActive,

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
    }
}
