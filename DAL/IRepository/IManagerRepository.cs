using DAL.Models;

namespace DAL.IRepository;

public interface IManagerRepository
{
    Task<bool> CheckUsernameExistsAsync(string username);
    Task<User> CreateManagerWithTransactionAsync(User user);
    Task<User> UpdateManagerWithTransactionAsync(User user);
    Task<List<User>> GetAllManagersAsync();
    Task<User?> GetManagerByIdAsync(int id);
    Task<User?> GetManagerForUpdateAsync(int id);
    Task<bool> DeleteManagerWithTransactionAsync(User user);
}