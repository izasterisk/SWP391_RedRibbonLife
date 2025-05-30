using BLL.DTO;

namespace BLL.Interfaces
{
    public interface ILoginService
    {
        Task<UserReadonlyDTO?> ValidateUserAsync(string username, string password);
    }
} 