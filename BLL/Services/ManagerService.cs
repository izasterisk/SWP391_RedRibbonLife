using AutoMapper;
using BLL.DTO.Manager;
using BLL.Interfaces;
using BLL.Utils;
using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class ManagerService : IManagerService
{
    private readonly IMapper _mapper;
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    private readonly IUserRepository<User> _userRepository;
    private readonly IUserUtils _userUtils;
    
    public ManagerService(IMapper mapper, SWP391_RedRibbonLifeContext dbContext, IUserRepository<User> userRepository, IUserUtils userUtils)
    {
        _mapper = mapper;
        _dbContext = dbContext;
        _userRepository = userRepository;
        _userUtils = userUtils;
    }
    
    public async Task<ManagerReadOnlyDTO> CreateManagerAsync(ManagerDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var usernameExists = await _userRepository.AnyAsync(u => u.Username.Equals(dto.Username));
            if (usernameExists)
            {
                throw new Exception($"Username {dto.Username} already exists.");
            }
            // Check if email already exists
            await _userUtils.CheckEmailExistAsync(dto.Email);
            User user = _mapper.Map<User>(dto);
            user.IsActive = true;
            user.UserRole = "Manager";
            user.IsVerified = true;
            user.Password = _userUtils.CreatePasswordHash(dto.Password);
            var createdManager = await _userRepository.CreateAsync(user);
            await transaction.CommitAsync();
            return _mapper.Map<ManagerReadOnlyDTO>(createdManager);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    public async Task<ManagerReadOnlyDTO> UpdateManagerAsync(ManagerUpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        await _userUtils.CheckUserExistAsync(dto.UserId);
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var manager = await _userRepository.GetAsync(u => u.UserId == dto.UserId, true);
            if (manager == null)
            {
                throw new Exception("Manager not found.");
            }
            if(manager.UserRole != "Manager")
            {
                throw new Exception("This account is not manager.");
            }
            if(!string.IsNullOrWhiteSpace(dto.Email))
            {
                if (dto.Email == manager.Email)
                {
                    throw new Exception("You are entering the exact email in your account");
                }
                await _userUtils.CheckEmailExistAsync(dto.Email);
            }
            // Update manager
            _mapper.Map(dto, manager);
            var updatedManager = await _userRepository.UpdateAsync(manager);
            await transaction.CommitAsync();
            return _mapper.Map<ManagerReadOnlyDTO>(updatedManager);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<List<ManagerReadOnlyDTO>> GetAllManagersAsync()
    {
        var managers = await _userRepository.GetAllByFilterAsync(u => u.IsActive && u.UserRole == "Manager", true);
        return _mapper.Map<List<ManagerReadOnlyDTO>>(managers);
    }
    
    public async Task<ManagerReadOnlyDTO> GetManagerByIdAsync(int id)
    {
        var manager = await _userRepository.GetAsync(u => u.IsActive && u.UserId == id, true);
        if (manager == null)
        {
            throw new Exception("Manager not found.");
        }
        return _mapper.Map<ManagerReadOnlyDTO>(manager);
    }
    
    public async Task<bool> DeleteManagerByIdAsync(int id)
    {
        var manager = await _userRepository.GetAsync(u => u.UserId == id, true);
        if (manager == null)
        {
            throw new Exception("Manager not found.");
        }
        if(manager.UserRole != "Manager")
        {
            throw new Exception("This account is not manager.");
        }
        await _userRepository.DeleteAsync(manager);
        return true;
    }
}
