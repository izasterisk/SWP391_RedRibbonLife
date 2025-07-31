using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository;

public class ManagerRepository : IManagerRepository
{
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    private readonly IRepository<User> _userRepository;

    public ManagerRepository(SWP391_RedRibbonLifeContext dbContext, IRepository<User> userRepository)
    {
        _dbContext = dbContext;
        _userRepository = userRepository;
    }

    public async Task<bool> CheckUsernameExistsAsync(string username)
    {
        return await _userRepository.AnyAsync(u => u.Username.Equals(username));
    }

    public async Task<User> CreateManagerWithTransactionAsync(User user)
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

    public async Task<User> UpdateManagerWithTransactionAsync(User user)
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

    public async Task<List<User>> GetAllManagersAsync()
    {
        return await _userRepository.GetAllByFilterAsync(u => u.IsActive && u.UserRole == "Manager", true);
    }

    public async Task<User?> GetManagerByIdAsync(int id)
    {
        return await _userRepository.GetAsync(u => u.IsActive && u.UserId == id, true);
    }

    public async Task<User?> GetManagerForUpdateAsync(int id)
    {
        return await _userRepository.GetAsync(u => u.UserId == id, false);
    }

    public async Task<bool> DeleteManagerWithTransactionAsync(User user)
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