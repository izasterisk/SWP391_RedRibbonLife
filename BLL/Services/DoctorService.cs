using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BLL.DTO;
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
    }
}
