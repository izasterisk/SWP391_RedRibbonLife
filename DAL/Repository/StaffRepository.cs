using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository;

public class StaffRepository : IStaffRepository
{
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    private readonly IRepository<User> _userRepository;

    public StaffRepository(SWP391_RedRibbonLifeContext dbContext, IRepository<User> userRepository)
    {
        _dbContext = dbContext;
        _userRepository = userRepository;
    }

    public async Task<bool> CheckUsernameExistsAsync(string username)
    {
        return await _userRepository.AnyAsync(u => u.Username.Equals(username));
    }

    public async Task<User> CreateStaffWithTransactionAsync(User user)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            user.Email = user.Email.ToLowerInvariant();
            var createdUser = await _userRepository.CreateAsync(user);
            await transaction.CommitAsync();
            return createdUser;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<User> UpdateStaffWithTransactionAsync(User user)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var updatedUser = await _userRepository.UpdateAsync(user);
            await transaction.CommitAsync();
            return updatedUser;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<User>> GetAllStaffsAsync()
    {
        return await _userRepository.GetAllByFilterAsync(u => u.IsActive && u.UserRole == "Staff", true);
    }

    public async Task<User?> GetStaffByIdAsync(int id, bool useNoTracking = true)
    {
        return await _userRepository.GetAsync(u => u.IsActive && u.UserId == id, useNoTracking);
    }

    public async Task<User?> GetStaffForUpdateAsync(int id)
    {
        return await _userRepository.GetAsync(u => u.UserId == id, false);
    }

    public async Task<bool> DeleteStaffWithTransactionAsync(User user)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            await _userRepository.DeleteAsync(user);
            await transaction.CommitAsync();
            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}