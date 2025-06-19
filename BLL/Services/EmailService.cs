using BLL.Interfaces;
using BLL.Utils;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using DAL.IRepository;
using DAL.Models;

namespace BLL.Services
{
    public class EmailService : IEmailService
    {
        private readonly SendGridEmailUtil _sendGridUtil;
        private readonly IUserRepository<User> _userRepository;
        private static readonly ConcurrentDictionary<string, string> _verificationCodes = new();

        public EmailService(IConfiguration configuration, IUserRepository<User> userRepository)
        {
            _sendGridUtil = new SendGridEmailUtil(configuration);
            _userRepository = userRepository;
        }

        public async Task<bool> SendEmailAsync(string email)
        {
            try
            {
                // Find user by email who is not verified
                var user = await _userRepository.GetAsync(u => u.Email.Equals(email) && !u.IsVerified);
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
                var user = await _userRepository.GetAsync(u => u.Email.Equals(email));
                if (user == null)
                {
                    throw new Exception("User not found");
                }
                // Update user verification status
                user.IsVerified = true;
                await _userRepository.UpdateAsync(user);
                // Remove verification code after successful verification
                _verificationCodes.TryRemove(email, out _);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to verify patient: {ex.Message}", ex);
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