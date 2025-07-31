using DAL.Models;

namespace DAL.IRepository
{
    public interface IAdminRepository
    {
        Task<User> CreateAdminWithTransactionAsync(User admin);
        Task<User> UpdateAdminWithTransactionAsync(User admin);
        Task<bool> DeleteAdminWithTransactionAsync(User admin);
        Task<User?> GetAdminByUserIdAsync(int userId);
        Task<User?> GetAdminByUserIdForUpdateAsync(int userId);
        Task<List<User>> GetAllAdminsAsync();
        Task<bool> IsUsernameExistsAsync(string username);
    }
}