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
    private readonly IStaffRepository _staffRepository;
    private readonly IUserUtils _userUtils;
    
    public StaffService(IMapper mapper, IStaffRepository staffRepository, IUserUtils userUtils)
    {
        _mapper = mapper;
        _staffRepository = staffRepository;
        _userUtils = userUtils;
    }
    
    public async Task<StaffReadOnlyDTO> CreateStaffAsync(StaffDTO dto)
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
        
        var usernameExists = await _staffRepository.CheckUsernameExistsAsync(dto.Username);
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
        
        var createdStaff = await _staffRepository.CreateStaffWithTransactionAsync(user);
        return _mapper.Map<StaffReadOnlyDTO>(createdStaff);
    }
    
    public async Task<StaffReadOnlyDTO> UpdateStaffAsync(StaffUpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        await _userUtils.CheckUserExistAsync(dto.UserId);
        
        var staff = await _staffRepository.GetStaffForUpdateAsync(dto.UserId);
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
        var updatedStaff = await _staffRepository.UpdateStaffWithTransactionAsync(staff);
        return _mapper.Map<StaffReadOnlyDTO>(updatedStaff);
    }
    
    public async Task<List<StaffReadOnlyDTO>> GetAllStaffsAsync()
    {
        var staffs = await _staffRepository.GetAllStaffsAsync();
        return _mapper.Map<List<StaffReadOnlyDTO>>(staffs);
    }
    
    public async Task<StaffReadOnlyDTO> GetStaffByIdAsync(int id)
    {
        var staff = await _staffRepository.GetStaffByIdAsync(id, true);
        if (staff == null)
        {
            throw new Exception("Staff not found.");
        }
        return _mapper.Map<StaffReadOnlyDTO>(staff);
    }
    
    public async Task<bool> DeleteStaffByIdAsync(int id)
    {
        var staff = await _staffRepository.GetStaffForUpdateAsync(id);
        if (staff == null)
        {
            throw new Exception("Staff not found.");
        }
        if(staff.UserRole != "Staff")
        {
            throw new Exception("This account is not staff.");
        }
        return await _staffRepository.DeleteStaffWithTransactionAsync(staff);
    }
}
