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
        private readonly IPatientService _patientService;
        private readonly IDoctorService _doctorService;
        private readonly IStaffService _staffService;
        private readonly IManagerService _managerService;
        private readonly IAdminService _adminService;

        public LoginService(IMapper mapper, IUserRepository<User> userRepository, IUserService userService, 
            IUserRepository<Patient> patientRepository, IUserRepository<Doctor> doctorRepository,
            IPatientService patientService, IDoctorService doctorService,
            IStaffService staffService, IManagerService managerService, IAdminService adminService)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _userService = userService;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
            _patientService = patientService;
            _doctorService = doctorService;
            _staffService = staffService;
            _managerService = managerService;
            _adminService = adminService;
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
                throw new Exception("User not found or this account has been deactivated.");
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
                        return await _patientService.GetPatientByPatientIDAsync(patient.PatientId);
                    }
                    break;
                case "Doctor":
                    var doctor = await _doctorRepository.GetAsync(d => d.UserId == user.UserId);
                    if (doctor != null)
                    {
                        return await _doctorService.GetDoctorByDoctorIDAsync(doctor.DoctorId);
                    }
                    break;
                case "Staff":
                    return await _staffService.GetStaffByIdAsync(user.UserId);
                case "Manager":
                    return await _managerService.GetManagerByIdAsync(user.UserId);
                case "Admin":
                    return await _adminService.GetAdminByIdAsync(user.UserId);
            }
            return null;
        }
    }
} 