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
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentNullException(nameof(dto.Email), "Email is required.");

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
            user.IsVerified = true; // Default
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

        public async Task<bool> UpdateDoctorAsync(DoctorUpdateDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");

            // Validate required fields
            //if (string.IsNullOrWhiteSpace(dto.Username))
            //    throw new ArgumentNullException(nameof(dto.Username), "Username is required.");
            //if (string.IsNullOrWhiteSpace(dto.FullName))
            //    throw new ArgumentNullException(nameof(dto.FullName), "Full name is required.");
            
            // Get existing doctor
            var doctor = await _doctorRepository.GetAsync(d => d.DoctorId == dto.DoctorId, true);
            if (doctor == null)
            {
                throw new Exception("Doctor not found.");
            }
            var user = await _userRepository.GetAsync(u => u.UserId == doctor.UserId);
            if (user == null)
            {
                throw new Exception($"User associated with Doctor ID {doctor.UserId} not found.");
            }
            
            // Update User entity
            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                user.PhoneNumber = dto.PhoneNumber;
            if (!string.IsNullOrWhiteSpace(dto.FullName))
                user.FullName = dto.FullName;
            if (dto.DateOfBirth != null)
                user.DateOfBirth = dto.DateOfBirth;
            if (!string.IsNullOrWhiteSpace(dto.Gender))
                user.Gender = dto.Gender;
            if (!string.IsNullOrWhiteSpace(dto.Address))
                user.Address = dto.Address;
            //user.IsVerified = dto.IsVerified;

            // Update Doctor entity
            if (!string.IsNullOrWhiteSpace(dto.DoctorImage))
                doctor.DoctorImage = dto.DoctorImage;
            if (!string.IsNullOrWhiteSpace(dto.Bio))
                doctor.Bio = dto.Bio;
            // Save changes
            await _userRepository.UpdateAsync(user);
            await _doctorRepository.UpdateAsync(doctor);
            return true;
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
                        UserRole = user.UserRole,
                        IsActive = user.IsActive,
                        IsVerified = user.IsVerified,

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
                UserRole = user.UserRole,
                IsActive = user.IsActive,
                IsVerified = user.IsVerified,

                // Doctor properties
                DoctorId = doctor.DoctorId,
                DoctorImage = doctor.DoctorImage,
                Bio = doctor.Bio
            };
            return doctorReadOnlyDTO;
        }
    }
}
