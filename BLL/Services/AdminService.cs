using AutoMapper;
using BLL.DTO.Admin;
using BLL.DTO.User;
using BLL.Interfaces;
using DAL.IRepository;
using DAL.Models;

namespace BLL.Services;

public class AdminService : IAdminService
{
    private readonly IUserRepository<User> _userRepository;
    private readonly IMapper _mapper;
    private readonly IUserUtils _userUtils;
    public AdminService(IUserRepository<User> userRepository, IMapper mapper, IUserUtils userUtils)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _userUtils = userUtils;
    }
    public async Task<dynamic> CreateAdminAsync(AdminDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        if (string.IsNullOrWhiteSpace(dto.Username))
            throw new ArgumentNullException(nameof(dto.Username), "Username is required.");
        if (string.IsNullOrWhiteSpace(dto.Password))
            throw new ArgumentNullException(nameof(dto.Password), "Password is required.");
        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new ArgumentNullException(nameof(dto.Email), "Email is required.");
        if (string.IsNullOrWhiteSpace(dto.FullName))
            throw new ArgumentNullException(nameof(dto.FullName), "Full name is required.");
        // Check if username already exists
        var existingUser = await _userRepository.GetAsync(u => u.Username.Equals(dto.Username));
        if (existingUser != null)
        {
            throw new Exception($"Username {dto.Username} already exists.");
        }
        // Check if email already exists
        var existingUserByEmail = await _userRepository.GetAsync(u => u.Email.Equals(dto.Email));
        if (existingUserByEmail != null)
        {
            throw new Exception($"Email {dto.Email} already exists.");
        }
        // Create User entity
        User user = _mapper.Map<User>(dto);
        user.IsActive = true; // Set default value for IsActive
        user.UserRole = "Admin"; // Set default value for UserRole
        user.IsVerified = true; // Default
        user.Password = _userUtils.CreatePasswordHash(dto.Password);
        // Save
        var createdAdmin = await _userRepository.CreateAsync(user);
        var adminDto = _mapper.Map<UserReadonlyDTO>(createdAdmin);
        return new
        {
            UserInfo = adminDto
        };
    }
    public async Task<dynamic> UpdateAdminAsync(AdminUpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        // Get existing admin
        var admin = await _userRepository.GetAsync(u => u.UserId == dto.UserId, true);
        if (admin == null)
        {
            throw new Exception("Admin not found.");
        }
        if(admin.UserRole != "Admin")
        {
            throw new Exception("This account is not admin.");
        }
        if(!string.IsNullOrWhiteSpace(dto.Email))
        {
            var existingUserByEmail = await _userRepository.GetAsync(u => u.Email.Equals(dto.Email) && u.UserId != dto.UserId);
            if (existingUserByEmail != null)
            {
                throw new Exception($"Email {dto.Email} already exists.");
            }
        }
        // Update admin
        _mapper.Map(dto, admin);
        await _userRepository.UpdateAsync(admin);
        var adminDto = _mapper.Map<UserReadonlyDTO>(admin);
        return new
        {
            UserInfo = adminDto
        };
    }
    public async Task<List<AdminReadOnlyDTO>> GetAllAdminsAsync()
    {
        var admins = await _userRepository.GetAllByFilterAsync(u => u.IsActive && u.UserRole == "Admin", true);
        return _mapper.Map<List<AdminReadOnlyDTO>>(admins);
    }
    public async Task<AdminReadOnlyDTO> GetAdminByAdminIDAsync(int id)
    {
        var admin = await _userRepository.GetAsync(u => u.IsActive && u.UserId == id, true);
        if (admin == null)
        {
            throw new Exception("Admin not found.");
        }
        return _mapper.Map<AdminReadOnlyDTO>(admin);
    }
    public async Task<bool> DeleteAdminAsync(int id)
    {
        var admin = await _userRepository.GetAsync(u => u.UserId == id, true);
        if (admin == null)
        {
            throw new Exception("Admin not found.");
        }
        if(admin.UserRole != "Admin")
        {
            throw new Exception("This account is not admin.");
        }
        await _userRepository.DeleteAsync(admin);
        return true;
    }
}