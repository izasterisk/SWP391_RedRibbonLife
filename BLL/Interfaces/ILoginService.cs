using BLL.DTO.User;

namespace BLL.Interfaces
{
    public interface ILoginService
    {
        Task<UserReadonlyDTO?> ValidateUserAsync(string username, string password);
    }
} 