using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BLL.DTO.Login;
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
        private readonly IUserService _userService;

        public LoginService(IUserRepository<User> userRepository, IMapper mapper, IUserService userService)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _userService = userService;
        }

        public async Task<UserReadonlyDTO?> ValidateUserAsync(string username, string password)
        {
            ArgumentNullException.ThrowIfNull(username, $"{nameof(username)} is null");
            ArgumentNullException.ThrowIfNull(password, $"{nameof(password)} is null");
            // Tìm user theo username, isActive = true và isVerified = true
            var user = await _userRepository.GetAsync(u => u.Username.Equals(username) && u.IsActive && u.IsVerified);
            if (user == null)
            {
                return null; // User không tồn tại hoặc chưa được verify
            }
            // Kiểm tra password sử dụng method từ UserService
            var hashedPassword = _userService.CreatePasswordHash(password);
            if (user.Password.Equals(hashedPassword))
            {
                return _mapper.Map<UserReadonlyDTO>(user);
            }
            return null; // Password không đúng
        }
        public async Task<bool> ChangePasswordAsync(ChangePasswordDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
            // Tìm user
            var user = await _userRepository.GetAsync(u => u.Email.Equals(dto.Email) && u.IsActive);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            // Kiểm tra password
            var hashedOldPassword = _userService.CreatePasswordHash(dto.Password);
            if (!user.Password.Equals(hashedOldPassword))
            {
                throw new Exception("Old password is incorrect.");
            }
            // Update password
            user.Password = _userService.CreatePasswordHash(dto.newPassword);
            await _userRepository.UpdateAsync(user);
            return true;
        }
        
    }
} 