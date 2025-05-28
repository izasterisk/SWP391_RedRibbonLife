using BLL.DTO;

namespace BLL.Interfaces
{
    public interface IUserService
    {
        string CreatePasswordHash(string password);
        Task<bool> CreateUserAsync(UserDTO dto);
    }
}
