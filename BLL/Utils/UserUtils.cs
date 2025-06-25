using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Interfaces;
using DAL.IRepository;
using DAL.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace BLL.Utils
{
    public class UserUtils : IUserUtils
    {
        private readonly IUserRepository<Appointment> _appointmentRepository;
        private readonly IUserRepository<User> _userRepository;
        private readonly IUserRepository<Patient> _patientRepository;
        private readonly IUserRepository<Doctor> _doctorRepository;
        private readonly IUserRepository<TestType> _testTypeRepository;
        private readonly IMapper _mapper;
        private readonly SWP391_RedRibbonLifeContext _dbContext;
        public UserUtils(IUserRepository<Appointment> appointmentRepository, IUserRepository<User> userRepository, IUserRepository<Patient> patientRepository, IUserRepository<Doctor> doctorRepository, IUserRepository<TestType> testTypeRepository, IMapper mapper, SWP391_RedRibbonLifeContext dbContext)
        {
            _appointmentRepository = appointmentRepository;
            _userRepository = userRepository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
            _testTypeRepository = testTypeRepository;
            _mapper = mapper;
            _dbContext = dbContext;
        }
        public string CreatePasswordHash(string password)
        {
            // Tạo hash từ password
            var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Array.Empty<byte>(), // Không sử dụng salt
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            ));
            return hash;
        }
        
        public void CheckDoctorExist(int doctorId)
        {
            // Check if doctor exists
            var doctor = _doctorRepository.GetAsync(u => u.DoctorId == doctorId, true).GetAwaiter().GetResult();
            if (doctor == null)
            {
                throw new Exception("Doctor not found.");
            }
        }
        
        public void CheckPatientExist(int patientId)
        {
            // Check if patient exists
            var patient = _patientRepository.GetAsync(u => u.PatientId == patientId, true).GetAwaiter().GetResult();
            if (patient == null)
            {
                throw new Exception("Patient not found.");
            }
        }
        
        public void CheckUserExist(int userId)
        {
            // Check if user exists
            var user = _userRepository.GetAsync(u => u.UserId == userId, true).GetAwaiter().GetResult();
            if (user == null)
            {
                throw new Exception("User not found.");
            }
        }
        
        public void CheckAppointmentExist(int appointmentId)
        {
            // Check if appointment exists
            var appointment = _appointmentRepository.GetAsync(a => a.AppointmentId == appointmentId, true).GetAwaiter().GetResult();
            if (appointment == null)
            {
                throw new Exception("Appointment not found.");
            }
        }
        
        public void CheckTestTypeExist(int testTypeId)
        {
            // Check if test type exists
            var testType = _testTypeRepository.GetAsync(t => t.TestTypeId == testTypeId, true).GetAwaiter().GetResult();
            if (testType == null)
            {
                throw new Exception("Test type not found.");
            }
        }
        
        public void ValidateEndTimeStartTime(TimeOnly startTime, TimeOnly endTime)
        {
            if (startTime >= endTime)
            {
                throw new ArgumentException("Start time must be earlier than end time.", nameof(startTime));
            }
        }
    }
}
