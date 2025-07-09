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
using Microsoft.EntityFrameworkCore;

namespace BLL.Utils
{
    public class UserUtils : IUserUtils
    {
        private readonly IUserRepository<Appointment> _appointmentRepository;
        private readonly IUserRepository<User> _userRepository;
        private readonly IUserRepository<Patient> _patientRepository;
        private readonly IUserRepository<Doctor> _doctorRepository;
        private readonly IUserRepository<TestType> _testTypeRepository;
        private readonly IUserRepository<TestResult> _testResultRepository;
        private readonly IUserRepository<Treatment> _treatmentRepository;
        private readonly IMapper _mapper;
        private readonly SWP391_RedRibbonLifeContext _dbContext;
        public UserUtils(IUserRepository<Appointment> appointmentRepository, IUserRepository<User> userRepository, IUserRepository<Patient> patientRepository, IUserRepository<Doctor> doctorRepository, IUserRepository<TestType> testTypeRepository, IUserRepository<TestResult> testResultRepository, IUserRepository<Treatment> treatmentRepository, IMapper mapper, SWP391_RedRibbonLifeContext dbContext)
        {
            _appointmentRepository = appointmentRepository;
            _userRepository = userRepository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
            _testTypeRepository = testTypeRepository;
            _testResultRepository = testResultRepository;
            _treatmentRepository = treatmentRepository;
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
        
        public async Task CheckDoctorExistAsync(int doctorId)
        {
            var doctor = await _doctorRepository.GetAsync(u => u.DoctorId == doctorId, true);
            if (doctor == null)
            {
                throw new Exception("Doctor not found.");
            }
        }
        
        public async Task CheckPatientExistAsync(int patientId)
        {
            var patient = await _patientRepository.GetWithRelationsAsync(u => u.PatientId == patientId, true,
                includeFunc: q => q.Include(t => t.User));
            if (patient == null)
            {
                throw new Exception("Patient not found.");
            }
            if (patient.User.IsActive == false)
            {
                throw new Exception("This account has been deactivated.");
            }
            if (patient.User.IsVerified == false)
            {
                throw new Exception("This account has not been verified.");
            }
        }
        
        public async Task CheckUserExistAsync(int userId)
        {
            var user = await _userRepository.GetAsync(u => u.UserId == userId, true);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
        }
        
        public async Task CheckAppointmentExistAsync(int appointmentId)
        {
            // Check if appointment exists
            var appointment = await _appointmentRepository.GetAsync(a => a.AppointmentId == appointmentId, true);
            if (appointment == null)
            {
                throw new Exception("Appointment not found.");
            }
        }
        public async Task CheckDuplicateAppointmentAsync(int appointmentId)
        {
            var duplicateAppointment = await _testResultRepository.GetAsync(a => a.AppointmentId == appointmentId, true);
            if (duplicateAppointment != null)
            {
                throw new Exception("1 appointment can only have 1 test result.");
            }
        }

        public async Task CheckTestTypeExistAsync(int testTypeId)
        {
            var testType = await _testTypeRepository.GetAsync(t => t.TestTypeId == testTypeId, true);
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
        
        public async Task CheckTestResultExistAsync(int id)
        {
            var testResult = await _testResultRepository.GetAsync(t => t.TestResultId == id, true);
            if (testResult == null)
            {
                throw new Exception("Test result not found.");
            }
            var treatments = await _treatmentRepository.GetAsync(t => t.TestResultId == id, true);
            if (treatments != null)
            {
                throw new Exception("1 test result can only link to 1 treatment.");
            }
        }
        
        public async Task CheckTreatmentExistAsync(int id)
        {
            var treatment = await _treatmentRepository.GetAsync(t => t.TreatmentId == id, true);
            if (treatment == null)
            {
                throw new Exception("Treatment not found.");
            }
        }
        
        public async Task CheckEmailExistAsync(string email)
        {
            var existingUser = await _userRepository.GetAsync(u => u.Email.Equals(email), true);
            if (existingUser != null)
            {
                throw new Exception($"Email {email} already exists.");
            }
        }
    }
}
