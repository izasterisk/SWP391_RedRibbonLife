using BLL.Interfaces;
using BLL.Utils;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class EmailService : IEmailService
    {
        private readonly SendGridEmailUtil _sendGridUtil;
        private readonly IRepository<User> _repository;
        private readonly IRepository<Treatment> _treatmentRepository;
        private static readonly ConcurrentDictionary<string, string> _verificationCodes = new();
        private readonly IUserUtils _userUtils;

        public EmailService(SendGridEmailUtil sendGridUtil, IRepository<User> repository, IRepository<Treatment> treatmentRepository, IUserUtils userUtils)
        {
            _sendGridUtil = sendGridUtil;
            _repository = repository;
            _treatmentRepository = treatmentRepository;
            _userUtils = userUtils;
        }

        public async Task<bool> SendVerificationEmailAsync(string email)
        {
            try
            {
                // Find user by email who is not verified
                var user = await _repository.GetAsync(u => u.Email.Equals(email) && !u.IsVerified);
                if (user == null)
                {
                    throw new Exception("User not found or already verified");
                }
                // Generate and store verification code
                var verificationCode = GenerateVerificationCode(email);
                // Send email
                await _sendGridUtil.SendVerificationEmailAsync(email, verificationCode);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send verification email: {ex.Message}", ex);
            }
        }
        
        public async Task<bool> SendTreatmentCreatedByTreatmentIdEmailAsync(int id)
        {
            try
            {
                var treatment = await _treatmentRepository.GetWithRelationsAsync(
                    filter: t => t.TreatmentId == id,
                    useNoTracking: true,
                    includeFunc: query => query
                        .Include(t => t.Regimen)
                            .ThenInclude(tr => tr.Component1)
                        .Include(t => t.Regimen)
                            .ThenInclude(tr => tr.Component2)
                        .Include(t => t.Regimen)
                            .ThenInclude(tr => tr.Component3)
                        .Include(t => t.Regimen)
                            .ThenInclude(tr => tr.Component4)
                        .Include(t => t.TestResult)
                            .ThenInclude(tr => tr.Patient)
                            .ThenInclude(p => p.User)
                        .Include(t => t.TestResult)
                            .ThenInclude(tr => tr.Doctor)
                            .ThenInclude(d => d.User)
                        .Include(t => t.TestResult)
                            .ThenInclude(tr => tr.Appointment)
                        .Include(t => t.TestResult)
                            .ThenInclude(tr => tr.TestType)
                );
                if (treatment == null)
                {
                    throw new Exception("Treatment not found.");
                }
                if (treatment.StartDate == null || treatment.EndDate == null)
                {
                    throw new Exception("Treatment start date or end date is null.");
                }
                if (treatment.Regimen == null)
                {
                    throw new Exception("Treatment regimen is null.");
                }
                if (treatment.TestResult == null)
                {
                    throw new Exception("Treatment test result is null.");
                }
                if (treatment.TestResult.Patient == null || treatment.TestResult.Patient.User == null)
                {
                    throw new Exception("Patient or patient user information is null.");
                }
                if (treatment.TestResult.Doctor == null || treatment.TestResult.Doctor.User == null)
                {
                    throw new Exception("Doctor or doctor user information is null.");
                }
                var components = new List<string>();
                if (treatment.Regimen.Component1?.ComponentName != null)
                    components.Add(treatment.Regimen.Component1.ComponentName);
                if (treatment.Regimen.Component2?.ComponentName != null)
                    components.Add(treatment.Regimen.Component2.ComponentName);
                if (treatment.Regimen.Component3?.ComponentName != null)
                    components.Add(treatment.Regimen.Component3.ComponentName);
                if (treatment.Regimen.Component4?.ComponentName != null)
                    components.Add(treatment.Regimen.Component4.ComponentName);
                string componentNames = components.Count > 0 ? string.Join(" + ", components) : "Các loại thuốc";
                await _sendGridUtil.SendTreatmentRegimenCreatedEmailAsync(
                    treatment.TestResult.Patient.User.Email,
                    treatment.TestResult.Patient.User.FullName ?? "Bệnh nhân",
                    treatment.TestResult.Doctor.User.FullName ?? "Bác sĩ",
                    treatment.Regimen.Frequency,
                    treatment.StartDate.Value,
                    treatment.EndDate.Value,
                    componentNames,
                    treatment.Notes ?? "Theo chỉ dẫn của bác sĩ"
                );
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send treatment created email: {ex.Message}", ex);
            }
        }

        public async Task<bool> VerifyPatientAsync(string email, string verifyCode)
        {
            try
            {
                // Check if verification code exists in dictionary
                if (!_verificationCodes.TryGetValue(email, out var storedCode) || storedCode != verifyCode)
                {
                    throw new Exception("Invalid verification code");
                }
                // Find user by email
                var user = await _repository.GetAsync(u => u.Email.Equals(email));
                if (user == null)
                {
                    throw new Exception("User not found");
                }
                // Update user verification status
                user.IsVerified = true;
                await _repository.UpdateAsync(user);
                // Remove verification code after successful verification
                _verificationCodes.TryRemove(email, out _);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to verify patient: {ex.Message}", ex);
            }
        }

        public async Task<bool> SendForgotPasswordEmailAsync(string email)
        {
            try
            {
                // Find user by email who is verified and active
                var user = await _repository.GetAsync(u => u.Email.Equals(email));
                if (user == null)
                {
                    throw new Exception("User not found");
                }
                // Generate and store verification code for password reset
                var verificationCode = GenerateVerificationCode(email);
                // Send password reset email
                await _sendGridUtil.SendForgotPasswordEmailAsync(email, verificationCode);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send forgot password email: {ex.Message}", ex);
            }
        }

        public async Task<bool> ChangePatientPasswordAsync(string email, string verifyCode, string newPassword)
        {
            try
            {
                // Check if verification code exists in dictionary
                if (!_verificationCodes.TryGetValue(email, out var storedCode) || storedCode != verifyCode)
                {
                    throw new Exception("Invalid verification code");
                }
                // Find user by email
                var user = await _repository.GetAsync(u => u.Email.Equals(email));
                if (user == null)
                {
                    throw new Exception("User not found");
                }
                // Update user password using UserUtils
                user.Password = _userUtils.CreatePasswordHash(newPassword);
                await _repository.UpdateAsync(user);
                // Remove verification code after successful password change
                _verificationCodes.TryRemove(email, out _);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to change password: {ex.Message}", ex);
            }
        }

        public string GenerateVerificationCode(string email)
        {
            var random = new Random();
            var code = random.Next(100000, 999999).ToString();
            // Store or update verification code for this email
            _verificationCodes.AddOrUpdate(email, code, (key, oldValue) => code);
            return code;
        }
    }
} 