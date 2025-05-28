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
using BLL.Utils;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository<User> _userRepository;
        public UserService(IUserRepository<User> userRepository, IMapper mapper)
        {
            _mapper = mapper;
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
        public async Task<bool> CreateUserAsync(UserDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
            var existingUser = await _userRepository.GetAsync(u => u.Username.Equals(dto.Username));
            if (existingUser != null)
            {
                throw new Exception($"Username {dto.Username} already exists.");
            }
            User user = _mapper.Map<User>(dto);
            user.IsActive = true; // Set default value for IsActive
            user.UserRole = "Customer"; // Set default value for UserRole
            if (!string.IsNullOrEmpty(dto.Password))
            {
                user.Password = CreatePasswordHash(dto.Password);
            }
            else
            {
                throw new ArgumentNullException(nameof(dto.Password), "Password cannot be null or empty.");
            }
            await _userRepository.CreateAsync(user);
            return true;
        }
    }
}
