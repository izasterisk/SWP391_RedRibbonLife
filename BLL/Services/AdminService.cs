using AutoMapper;
using BLL.DTO.Admin;
using BLL.Interfaces;
using BLL.Utils;
using DAL.IRepository;
using DAL.Models;

namespace BLL.Services;

public class AdminService : IAdminService
{
    private readonly IMapper _mapper;
    private readonly IAdminRepository _adminRepository;
    private readonly IUserUtils _userUtils;
    
    public AdminService(IMapper mapper, IAdminRepository adminRepository, IUserUtils userUtils)
    {
        _mapper = mapper;
        _adminRepository = adminRepository;
        _userUtils = userUtils;
    }
    
    public async Task<AdminReadOnlyDTO> CreateAdminAsync(AdminDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        var usernameExists = await _adminRepository.IsUsernameExistsAsync(dto.Username);
        if (usernameExists)
        {
            throw new Exception($"Username {dto.Username} already exists.");
        }
        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            await _userUtils.CheckEmailExistAsync(dto.Email);
        }
        User user = _mapper.Map<User>(dto);
        user.Password = _userUtils.CreatePasswordHash(dto.Password);
        var createdAdmin = await _adminRepository.CreateAdminWithTransactionAsync(user);
        return _mapper.Map<AdminReadOnlyDTO>(createdAdmin);
    }
    public async Task<AdminReadOnlyDTO> UpdateAdminAsync(AdminUpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        await _userUtils.CheckUserExistAsync(dto.UserId);
        var admin = await _adminRepository.GetAdminByUserIdForUpdateAsync(dto.UserId);
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
            if (dto.Email == admin.Email)
            {
                throw new Exception("You are entering the exact email in your account");
            }
            await _userUtils.CheckEmailExistAsync(dto.Email);
        }
        // Update admin
        _mapper.Map(dto, admin);
        var updatedAdmin = await _adminRepository.UpdateAdminWithTransactionAsync(admin);
        return _mapper.Map<AdminReadOnlyDTO>(updatedAdmin);
    }
    
    public async Task<List<AdminReadOnlyDTO>> GetAllAdminsAsync()
    {
        var admins = await _adminRepository.GetAllAdminsAsync();
        return _mapper.Map<List<AdminReadOnlyDTO>>(admins);
    }
    
    public async Task<AdminReadOnlyDTO> GetAdminByUserIdAsync(int id)
    {
        var admin = await _adminRepository.GetAdminByUserIdAsync(id);
        if (admin == null)
        {
            throw new Exception("Admin not found.");
        }
        return _mapper.Map<AdminReadOnlyDTO>(admin);
    }
    
    public async Task<bool> DeleteAdminByIdAsync(int id)
    {
        var admin = await _adminRepository.GetAdminByUserIdAsync(id);
        if (admin == null)
        {
            throw new Exception("Admin not found.");
        }
        return await _adminRepository.DeleteAdminWithTransactionAsync(admin);
    }
}