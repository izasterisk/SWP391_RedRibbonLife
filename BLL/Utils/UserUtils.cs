using System;
using BLL.Interfaces;
using DAL.IRepository;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace BLL.Utils
{
    public class UserUtils : IUserUtils
    {
        private readonly IUserRepository _userRepository;

        public UserUtils(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
        
        public void ValidateEndTimeStartTime(TimeOnly startTime, TimeOnly endTime)
        {
            if (startTime >= endTime)
            {
                throw new ArgumentException("Start time must be earlier than end time.", nameof(startTime));
            }
        }

        public async Task CheckDoctorExistAsync(int doctorId)
        {
            await _userRepository.CheckDoctorExistAsync(doctorId);
        }

        public async Task CheckPatientExistAsync(int patientId)
        {
            await _userRepository.CheckPatientExistAsync(patientId);
        }

        public async Task CheckUserExistAsync(int userId)
        {
            await _userRepository.CheckUserExistAsync(userId);
        }

        public async Task CheckAppointmentExistAsync(int appointmentId)
        {
            await _userRepository.CheckAppointmentExistAsync(appointmentId);
        }

        public async Task CheckTestTypeExistAsync(int testTypeId)
        {
            await _userRepository.CheckTestTypeExistAsync(testTypeId);
        }

        public async Task CheckDuplicateAppointmentAsync(int appointmentId)
        {
            await _userRepository.CheckDuplicateAppointmentAsync(appointmentId);
        }

        public async Task CheckTestResultExistAsync(int id)
        {
            await _userRepository.CheckTestResultExistAsync(id);
        }

        public async Task CheckTreatmentExistAsync(int id)
        {
            await _userRepository.CheckTreatmentExistAsync(id);
        }

        public async Task CheckEmailExistAsync(string email)
        {
            await _userRepository.CheckEmailExistAsync(email);
        }

        public async Task CheckCategoryExistAsync(int id)
        {
            await _userRepository.CheckCategoryExistAsync(id);
        }
    }
}
