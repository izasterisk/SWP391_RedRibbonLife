using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BLL.DTO.Doctor;
using BLL.DTO.Login;
using BLL.DTO.Patient;
using BLL.DTO.User;
using BLL.Interfaces;
using DAL.IRepository;
using DAL.Models;

namespace BLL.Services
{
    public class LoginService : ILoginService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository<User> _userRepository;
        private readonly IUserRepository<Patient> _patientRepository;
        private readonly IUserRepository<Doctor> _doctorRepository;
        private readonly IUserService _userService;

        public LoginService(IMapper mapper, IUserRepository<User> userRepository, IUserService userService, IUserRepository<Patient> patientRepository, IUserRepository<Doctor> doctorRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _userService = userService;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
        }

        public async Task<UserReadonlyDTO?> ValidateUserAsync(string username, string password)
        {
            ArgumentNullException.ThrowIfNull(username, $"{nameof(username)} is null");
            ArgumentNullException.ThrowIfNull(password, $"{nameof(password)} is null");
            var user = await _userRepository.GetAsync(u => u.Username.Equals(username) && u.IsActive && u.IsVerified);
            if (user == null)
            {
                return null;
            }
            var hashedPassword = _userService.CreatePasswordHash(password);
            if (user.Password.Equals(hashedPassword))
            {
                return _mapper.Map<UserReadonlyDTO>(user);
            }
            return null;
        }

        public async Task<object> ChangePasswordAsync(ChangePasswordDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
            var user = await _userRepository.GetAsync(u => u.Email.Equals(dto.Email) && u.IsActive);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            var hashedOldPassword = _userService.CreatePasswordHash(dto.Password);
            if (!user.Password.Equals(hashedOldPassword))
            {
                throw new Exception("Old password is incorrect.");
            }
            user.Password = _userService.CreatePasswordHash(dto.newPassword);
            var user1 = await _userRepository.UpdateAsync(user);
            switch (user.UserRole)
            {
                case "Patient":
                    var patient = await _patientRepository.GetAsync(p => p.UserId == user.UserId);
                    if (patient != null)
                    {
                        return new PatientReadOnlyDTO
                        {
                            // User properties
                            Username = user1.Username,
                            Email = user1.Email,
                            PhoneNumber = user1.PhoneNumber,
                            FullName = user1.FullName,
                            DateOfBirth = user1.DateOfBirth,
                            Gender = user1.Gender,
                            Address = user1.Address,
                            // Patient properties
                            PatientId = patient.PatientId,
                            BloodType = patient.BloodType,
                            IsPregnant = patient.IsPregnant,
                            SpecialNotes = patient.SpecialNotes
                        };
                    }
                    break;
                case "Doctor":
                    var doctor = await _doctorRepository.GetAsync(d => d.UserId == user.UserId);
                    if (doctor != null)
                    {
                        return new DoctorReadOnlyDTO
                        {
                            // User properties
                            Username = user1.Username,
                            Email = user1.Email,
                            PhoneNumber = user1.PhoneNumber,
                            FullName = user1.FullName,
                            DateOfBirth = user1.DateOfBirth,
                            Gender = user1.Gender,
                            Address = user1.Address,
                            // Doctor properties
                            DoctorId = doctor.DoctorId,
                            DoctorImage = doctor.DoctorImage,
                            Bio = doctor.Bio
                        };
                    }
                    break;
            }
            return null;
        }
    }
} 