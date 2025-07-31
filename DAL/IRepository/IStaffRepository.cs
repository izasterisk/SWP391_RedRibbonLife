using DAL.Models;

namespace DAL.IRepository;

public interface IStaffRepository
{
    Task<bool> CheckUsernameExistsAsync(string username);
    Task<User> CreateStaffWithTransactionAsync(User user);
    Task<User> UpdateStaffWithTransactionAsync(User user);
    Task<List<User>> GetAllStaffsAsync();
    Task<User?> GetStaffByIdAsync(int id, bool useNoTracking = true);
    Task<User?> GetStaffForUpdateAsync(int id);
    Task<bool> DeleteStaffWithTransactionAsync(User user);
}