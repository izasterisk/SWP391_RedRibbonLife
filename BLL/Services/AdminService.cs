using AutoMapper;
using BLL.DTO.Admin;
using BLL.Interfaces;
using BLL.Utils;
using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class AdminService : IAdminService
{
    private readonly IMapper _mapper;
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    private readonly IUserRepository<User> _userRepository;
    private readonly IUserUtils _userUtils;
    
    public AdminService(IMapper mapper, SWP391_RedRibbonLifeContext dbContext, IUserRepository<User> userRepository, IUserUtils userUtils)
    {
        _mapper = mapper;
        _dbContext = dbContext;
        _userRepository = userRepository;
        _userUtils = userUtils;
    }
    
    public async Task<AdminReadOnlyDTO> CreateAdminAsync(AdminDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var existingUser = await _userRepository.GetAsync(u => u.Username.Equals(dto.Username));
            if (existingUser != null)
            {
                throw new Exception($"Username {dto.Username} already exists.");
            }
            // Check if email already exists
            await _userUtils.CheckEmailExistAsync(dto.Email);
            User user = _mapper.Map<User>(dto);
            user.IsActive = true;
            user.UserRole = "Admin";
            user.IsVerified = true;
            user.Password = _userUtils.CreatePasswordHash(dto.Password);
            var createdAdmin = await _userRepository.CreateAsync(user);
            await transaction.CommitAsync();
            return _mapper.Map<AdminReadOnlyDTO>(createdAdmin);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    public async Task<AdminReadOnlyDTO> UpdateAdminAsync(AdminUpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        await _userUtils.CheckUserExistAsync(dto.UserId);
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
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
                if (dto.Email == admin.Email)
                {
                    throw new Exception("You are entering the exact email in your account");
                }
                await _userUtils.CheckEmailExistAsync(dto.Email);
            }
            // Update admin
            _mapper.Map(dto, admin);
            var updatedAdmin = await _userRepository.UpdateAsync(admin);
            await transaction.CommitAsync();
            return _mapper.Map<AdminReadOnlyDTO>(updatedAdmin);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<List<AdminReadOnlyDTO>> GetAllAdminsAsync()
    {
        var admins = await _userRepository.GetAllByFilterAsync(u => u.IsActive && u.UserRole == "Admin", true);
        return _mapper.Map<List<AdminReadOnlyDTO>>(admins);
    }
    
    public async Task<AdminReadOnlyDTO> GetAdminByUserIdAsync(int id)
    {
        var admin = await _userRepository.GetAsync(u => u.IsActive && u.UserId == id, true);
        if (admin == null)
        {
            throw new Exception("Admin not found.");
        }
        return _mapper.Map<AdminReadOnlyDTO>(admin);
    }
    
    public async Task<bool> DeleteAdminByIdAsync(int id)
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