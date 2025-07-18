using BLL.DTO.Login;
using BLL.DTO.User;

namespace BLL.Interfaces
{
    public interface ILoginService
    {
        Task<UserReadonlyDTO?> ValidateUserAsync(string username, string password);
        Task<object> ChangePasswordAsync(ChangePasswordDTO dto);
        Task<object> LoginServiceAsync(LoginDTO dto);
        Task<object> GetMeAsync(int userId);
    }
} 