using BLL.DTO.Manager;

namespace BLL.Interfaces;

public interface IManagerService
{
    Task<ManagerReadOnlyDTO> CreateManagerAsync(ManagerDTO dto);
    Task<ManagerReadOnlyDTO> UpdateManagerAsync(ManagerUpdateDTO dto);
    Task<List<ManagerReadOnlyDTO>> GetAllManagersAsync();
    Task<ManagerReadOnlyDTO> GetManagerByIdAsync(int id);
    Task<bool> DeleteManagerByIdAsync(int id);
}