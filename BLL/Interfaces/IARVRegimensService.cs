using BLL.DTO.ARVRegimens;

namespace BLL.Interfaces;

public interface IARVRegimensService
{
    Task<ARVRegimensReadOnlyDTO> CreateARVRegimensAsync(ARVRegimensCreateDTO dto);
    Task<ARVRegimensReadOnlyDTO> UpdateARVRegimensAsync(ARVRegimensUpdateDTO dto);
    Task<List<ARVRegimensReadOnlyDTO>> GetAllARVRegimensAsync();
    Task<ARVRegimensReadOnlyDTO> GetARVRegimensByIdAsync(int id);
    Task<bool> DeleteARVRegimensAsync(int id);
}