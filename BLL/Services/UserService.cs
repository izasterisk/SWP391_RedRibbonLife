using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Interfaces;
using DAL.IRepository;
using DAL.Models;
using BLL.Utils;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using BLL.DTO.User;

namespace BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<User> _repository;
        public UserService(IRepository<User> repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
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
            var existingUser = await _repository.GetAsync(u => u.Username.Equals(dto.Username));
            if (existingUser != null)
            {
                throw new Exception($"Username {dto.Username} already exists.");
            }
            
            // Check if email already exists (only if email is provided)
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var existingUserByEmail = await _repository.GetAsync(u => u.Email.Equals(dto.Email));
                if (existingUserByEmail != null)
                {
                    throw new Exception($"Email {dto.Email} already exists.");
                }
            }
            
            User user = _mapper.Map<User>(dto);
            user.IsActive = true; // Set default value for IsActive
            user.UserRole = "Customer"; // Set default value for UserRole
            user.IsVerified = dto.IsVerified; // Set IsVerified from DTO
            user.Password = CreatePasswordHash(dto.Password);
            
            await _repository.CreateAsync(user);
            return true;
        }
        public async Task<List<UserReadonlyDTO>> GetAllUserAsync()
        {
            var users = await _repository.GetAllByFilterAsync(u => u.IsActive);
            return _mapper.Map<List<UserReadonlyDTO>>(users);
        }
        public async Task<UserReadonlyDTO> GetUserByFullnameAsync(string fullname)
        {
            ArgumentNullException.ThrowIfNull(fullname, $"{nameof(fullname)} is null");
            var user = await _repository.GetAsync(u => u.IsActive && u.FullName.Equals(fullname));
            if (user == null)
            {
                throw new KeyNotFoundException($"User with fullname {fullname} not found.");
            }
            return _mapper.Map<UserReadonlyDTO>(user);
        }
        public async Task<bool> UpdateUserAsync(UserDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
            var existingUser = await _repository.GetAsync(u => u.IsActive && u.UserId == dto.UserId, true);
            if (existingUser == null)
            {
                throw new KeyNotFoundException($"User with ID {dto.UserId} not found.");
            }
            var userWithSameUsername = await _repository.GetAsync(u => u.Username.Equals(dto.Username) && u.UserId != dto.UserId);
            if (userWithSameUsername != null)
            {
                throw new Exception($"Username {dto.Username} already exists.");
            }
            existingUser.Username = dto.Username;

            // Update other user properties
            existingUser.Email = dto.Email;
            existingUser.PhoneNumber = dto.PhoneNumber;
            existingUser.FullName = dto.FullName;
            existingUser.DateOfBirth = dto.DateOfBirth;
            existingUser.Gender = dto.Gender;
            existingUser.Address = dto.Address;
            existingUser.IsVerified = dto.IsVerified;
            
            // Only update password if provided
            if (!string.IsNullOrEmpty(dto.Password))
            {
                existingUser.Password = CreatePasswordHash(dto.Password);
            }
            
            await _repository.UpdateAsync(existingUser);
            return true;
        }
    }
}
