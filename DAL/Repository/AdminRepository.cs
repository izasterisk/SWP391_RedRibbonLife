using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository;

public class AdminRepository : IAdminRepository
{
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    private readonly IRepository<User> _userRepository;
    
    public AdminRepository(SWP391_RedRibbonLifeContext dbContext, IRepository<User> userRepository)
    {
        _dbContext = dbContext;
        _userRepository = userRepository;
    }

    public async Task<User> CreateAdminWithTransactionAsync(User admin)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            admin.IsActive = true;
            admin.UserRole = "Admin";
            admin.IsVerified = true;
            var createdAdmin = await _userRepository.CreateAsync(admin);
            await transaction.CommitAsync();
            return createdAdmin;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<User> UpdateAdminWithTransactionAsync(User admin)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var updatedAdmin = await _userRepository.UpdateAsync(admin);
            await transaction.CommitAsync();
            return updatedAdmin;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> DeleteAdminWithTransactionAsync(User admin)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var result = await _userRepository.DeleteAsync(admin);
            await transaction.CommitAsync();
            return result;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<User?> GetAdminByUserIdAsync(int userId)
    {
        return await _userRepository.GetAsync(u => u.IsActive && u.UserId == userId && u.UserRole == "Admin", useNoTracking: true);
    }

    public async Task<User?> GetAdminByUserIdForUpdateAsync(int userId)
    {
        return await _userRepository.GetAsync(u => u.IsActive && u.UserId == userId && u.UserRole == "Admin", useNoTracking: false);
    }

    public async Task<List<User>> GetAllAdminsAsync()
    {
        return await _userRepository.GetAllByFilterAsync(u => u.IsActive && u.UserRole == "Admin", useNoTracking: true);
    }

    public async Task<bool> IsUsernameExistsAsync(string username)
    {
        return await _userRepository.AnyAsync(u => u.Username.Equals(username));
    }
}