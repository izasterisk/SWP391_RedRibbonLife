using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
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
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

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
        private readonly IConfiguration _configuration;

        public LoginService(IMapper mapper, IUserRepository<User> userRepository, IUserService userService, 
            IUserRepository<Patient> patientRepository, IUserRepository<Doctor> doctorRepository,
            IPatientService patientService, IDoctorService doctorService,
            IStaffService staffService, IManagerService managerService, IAdminService adminService,
            IConfiguration configuration)
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
            _configuration = configuration;
        }

        public async Task<UserReadonlyDTO?> ValidateUserAsync(string username, string password)
        {
            ArgumentNullException.ThrowIfNull(username, $"{nameof(username)} is null");
            ArgumentNullException.ThrowIfNull(password, $"{nameof(password)} is null");
            var user = await _userRepository.GetAsync(u => u.Username.Equals(username) && u.IsActive);
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

        public async Task<object> LoginServiceAsync(LoginDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
            var user = await ValidateUserAsync(dto.Username, dto.Password);
            if (user == null)
            {
                throw new Exception("Invalid username or password");
            }
            // Generate JWT Token
            string audience = _configuration.GetValue<string>("LocalAudience");
            string issuer = _configuration.GetValue<string>("LocalIssuer");
            byte[] key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("JWTSecretforLocaluser"));
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Issuer = issuer,
                Audience = audience,
                Subject = new ClaimsIdentity(new Claim[]
                {
                    // UserId
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            // Get detailed user information based on role
            object userDetails = null;
            switch (user.UserRole)
            {
                case "Patient":
                    var patient = await _patientRepository.GetAsync(p => p.UserId == user.UserId);
                    if (patient != null)
                    {
                        userDetails = await _patientService.GetPatientByPatientIDAsync(patient.PatientId);
                    }
                    break;
                case "Doctor":
                    var doctor = await _doctorRepository.GetAsync(d => d.UserId == user.UserId);
                    if (doctor != null)
                    {
                        userDetails = await _doctorService.GetDoctorByDoctorIDAsync(doctor.DoctorId);
                    }
                    break;
                case "Staff":
                    userDetails = await _staffService.GetStaffByIdAsync(user.UserId);
                    break;
                case "Manager":
                    userDetails = await _managerService.GetManagerByIdAsync(user.UserId);
                    break;
                case "Admin":
                    userDetails = await _adminService.GetAdminByUserIdAsync(user.UserId);
                    break;
            }
            // Return combined response with user details and token
            return new
            {
                Token = tokenString,
                UserDetails = userDetails
            };
        }

        public async Task<object> GetMeAsync(int userId)
        {
            ArgumentNullException.ThrowIfNull(userId, $"{nameof(userId)} is null");
            var user = await _userRepository.GetAsync(u => u.UserId == userId && u.IsActive);
            if (user == null)
            {
                throw new Exception("User not found or this account has been deactivated.");
            }
            // Get detailed user information based on role
            object userDetails = null;
            switch (user.UserRole)
            {
                case "Patient":
                    var patient = await _patientRepository.GetAsync(p => p.UserId == user.UserId);
                    if (patient != null)
                    {
                        userDetails = await _patientService.GetPatientByPatientIDAsync(patient.PatientId);
                    }
                    break;
                case "Doctor":
                    var doctor = await _doctorRepository.GetAsync(d => d.UserId == user.UserId);
                    if (doctor != null)
                    {
                        userDetails = await _doctorService.GetDoctorByDoctorIDAsync(doctor.DoctorId);
                    }
                    break;
                case "Staff":
                    userDetails = await _staffService.GetStaffByIdAsync(user.UserId);
                    break;
                case "Manager":
                    userDetails = await _managerService.GetManagerByIdAsync(user.UserId);
                    break;
                case "Admin":
                    userDetails = await _adminService.GetAdminByUserIdAsync(user.UserId);
                    break;
            }
            return userDetails;
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
                    return await _adminService.GetAdminByUserIdAsync(user.UserId);
            }
            return null;
        }
    }
} 