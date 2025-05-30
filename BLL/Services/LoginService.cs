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

            // Tìm user theo username và isActive = true
            var user = await _userRepository.GetAsync(u => u.Username.Equals(username) && u.IsActive);
            
            if (user == null)
            {
                return null; // User không tồn tại
            }

            // Kiểm tra password sử dụng method từ UserService
            var hashedPassword = _userService.CreatePasswordHash(password);
            if (user.Password.Equals(hashedPassword))
            {
                return _mapper.Map<UserReadonlyDTO>(user);
            }

            return null; // Password không đúng
        }
    }
} 