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
    private readonly IRepository<User> _repository;
    private readonly IUserUtils _userUtils;
    
    public StaffService(IMapper mapper, SWP391_RedRibbonLifeContext dbContext, IRepository<User> repository, IUserUtils userUtils)
    {
        _mapper = mapper;
        _dbContext = dbContext;
        _repository = repository;
        _userUtils = userUtils;
    }
    
    public async Task<StaffReadOnlyDTO> CreateStaffAsync(StaffDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var usernameExists = await _repository.AnyAsync(u => u.Username.Equals(dto.Username));
            if (usernameExists)
            {
                throw new Exception($"Username {dto.Username} already exists.");
            }
            await _userUtils.CheckEmailExistAsync(dto.Email);
            User user = _mapper.Map<User>(dto);
            user.IsActive = true;
            user.UserRole = "Staff";
            user.IsVerified = true;
            user.Password = _userUtils.CreatePasswordHash(dto.Password);
            var createdStaff = await _repository.CreateAsync(user);
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
        await _userUtils.CheckUserExistAsync(dto.UserId);
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var staff = await _repository.GetAsync(u => u.UserId == dto.UserId, true);
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
                await _userUtils.CheckEmailExistAsync(dto.Email);
            }
            // Update staff
            _mapper.Map(dto, staff);
            var updatedStaff = await _repository.UpdateAsync(staff);
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
        var staffs = await _repository.GetAllByFilterAsync(u => u.IsActive && u.UserRole == "Staff", true);
        return _mapper.Map<List<StaffReadOnlyDTO>>(staffs);
    }
    
    public async Task<StaffReadOnlyDTO> GetStaffByIdAsync(int id)
    {
        var staff = await _repository.GetAsync(u => u.IsActive && u.UserId == id, true);
        if (staff == null)
        {
            throw new Exception("Staff not found.");
        }
        return _mapper.Map<StaffReadOnlyDTO>(staff);
    }
    
    public async Task<bool> DeleteStaffByIdAsync(int id)
    {
        var staff = await _repository.GetAsync(u => u.UserId == id, true);
        if (staff == null)
        {
            throw new Exception("Staff not found.");
        }
        if(staff.UserRole != "Staff")
        {
            throw new Exception("This account is not staff.");
        }
        await _repository.DeleteAsync(staff);
        return true;
    }
}
