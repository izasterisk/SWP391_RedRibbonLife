using BLL.DTO.User;

namespace BLL.Interfaces
{
    public interface IUserService
    {
        string CreatePasswordHash(string password);
        Task<bool> CreateUserAsync(UserDTO dto);
        Task<List<UserReadonlyDTO>> GetAllUserAsync();
        Task<UserReadonlyDTO> GetUserByFullnameAsync(string fullname);
        Task<bool> UpdateUserAsync(UserDTO dto);
    }
}
