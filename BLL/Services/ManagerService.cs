using AutoMapper;
using BLL.DTO.Manager;
using BLL.Interfaces;
using BLL.Utils;
using DAL.IRepository;
using DAL.Models;

namespace BLL.Services;

public class ManagerService : IManagerService
{
    private readonly IMapper _mapper;
    private readonly IManagerRepository _managerRepository;
    private readonly IUserUtils _userUtils;
    
    public ManagerService(IMapper mapper, IManagerRepository managerRepository, IUserUtils userUtils)
    {
        _mapper = mapper;
        _managerRepository = managerRepository;
        _userUtils = userUtils;
    }
    
    public async Task<ManagerReadOnlyDTO> CreateManagerAsync(ManagerDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        
        if (string.IsNullOrEmpty(dto.Username))
        {
            throw new ArgumentException("Username cannot be null or empty.");
        }
        if (string.IsNullOrEmpty(dto.Email))
        {
            throw new ArgumentException("Email cannot be null or empty.");
        }
        
        var usernameExists = await _managerRepository.CheckUsernameExistsAsync(dto.Username);
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
        
        var createdManager = await _managerRepository.CreateManagerWithTransactionAsync(user);
        return _mapper.Map<ManagerReadOnlyDTO>(createdManager);
    }
    
    public async Task<ManagerReadOnlyDTO> UpdateManagerAsync(ManagerUpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        await _userUtils.CheckUserExistAsync(dto.UserId);
        
        var manager = await _managerRepository.GetManagerForUpdateAsync(dto.UserId);
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
        var updatedManager = await _managerRepository.UpdateManagerWithTransactionAsync(manager);
        return _mapper.Map<ManagerReadOnlyDTO>(updatedManager);
    }
    
    public async Task<List<ManagerReadOnlyDTO>> GetAllManagersAsync()
    {
        var managers = await _managerRepository.GetAllManagersAsync();
        return _mapper.Map<List<ManagerReadOnlyDTO>>(managers);
    }
    
    public async Task<ManagerReadOnlyDTO> GetManagerByIdAsync(int id)
    {
        var manager = await _managerRepository.GetManagerByIdAsync(id);
        if (manager == null)
        {
            throw new Exception("Manager not found.");
        }
        return _mapper.Map<ManagerReadOnlyDTO>(manager);
    }
    
    public async Task<bool> DeleteManagerByIdAsync(int id)
    {
        var manager = await _managerRepository.GetManagerForUpdateAsync(id);
        if (manager == null)
        {
            throw new Exception("Manager not found.");
        }
        if(manager.UserRole != "Manager")
        {
            throw new Exception("This account is not manager.");
        }
        return await _managerRepository.DeleteManagerWithTransactionAsync(manager);
    }
}
