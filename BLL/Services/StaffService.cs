using AutoMapper;
using BLL.DTO.Staff;
using BLL.Interfaces;
using BLL.Utils;
using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class StaffService : IStaffService
{
    private readonly IMapper _mapper;
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    private readonly IUserRepository<User> _userRepository;
    private readonly IUserUtils _userUtils;
    
    public StaffService(IMapper mapper, SWP391_RedRibbonLifeContext dbContext, IUserRepository<User> userRepository, IUserUtils userUtils)
    {
        _mapper = mapper;
        _dbContext = dbContext;
        _userRepository = userRepository;
        _userUtils = userUtils;
    }
    
    public async Task<StaffReadOnlyDTO> CreateStaffAsync(StaffDTO dto)
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
            _userUtils.CheckEmailExist(dto.Email);
            User user = _mapper.Map<User>(dto);
            user.IsActive = true;
            user.UserRole = "Staff";
            user.IsVerified = true;
            user.Password = _userUtils.CreatePasswordHash(dto.Password);
            var createdStaff = await _userRepository.CreateAsync(user);
            await transaction.CommitAsync();
            return _mapper.Map<StaffReadOnlyDTO>(createdStaff);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    public async Task<StaffReadOnlyDTO> UpdateStaffAsync(StaffUpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        _userUtils.CheckUserExist(dto.UserId);
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var staff = await _userRepository.GetAsync(u => u.UserId == dto.UserId, true);
            if (staff == null)
            {
                throw new Exception("Staff not found.");
            }
            if(staff.UserRole != "Staff")
            {
                throw new Exception("This account is not staff.");
            }
            if(!string.IsNullOrWhiteSpace(dto.Email))
            {
                if (dto.Email == staff.Email)
                {
                    throw new Exception("You are entering the exact email in your account");
                }
                _userUtils.CheckEmailExist(dto.Email);
            }
            // Update staff
            _mapper.Map(dto, staff);
            var updatedStaff = await _userRepository.UpdateAsync(staff);
            await transaction.CommitAsync();
            return _mapper.Map<StaffReadOnlyDTO>(updatedStaff);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<List<StaffReadOnlyDTO>> GetAllStaffsAsync()
    {
        var staffs = await _userRepository.GetAllByFilterAsync(u => u.IsActive && u.UserRole == "Staff", true);
        return _mapper.Map<List<StaffReadOnlyDTO>>(staffs);
    }
    
    public async Task<StaffReadOnlyDTO> GetStaffByIdAsync(int id)
    {
        var staff = await _userRepository.GetAsync(u => u.IsActive && u.UserId == id, true);
        if (staff == null)
        {
            throw new Exception("Staff not found.");
        }
        return _mapper.Map<StaffReadOnlyDTO>(staff);
    }
    
    public async Task<bool> DeleteStaffByIdAsync(int id)
    {
        var staff = await _userRepository.GetAsync(u => u.UserId == id, true);
        if (staff == null)
        {
            throw new Exception("Staff not found.");
        }
        if(staff.UserRole != "Staff")
        {
            throw new Exception("This account is not staff.");
        }
        await _userRepository.DeleteAsync(staff);
        return true;
    }
}