using BLL.DTO.ARVRegimens;

namespace BLL.Interfaces;

public interface IARVRegimensService
{
    Task<dynamic> CreateARVRegimensAsync(ARVRegimensCreateDTO dto);
    Task<dynamic> UpdateARVRegimensAsync(ARVRegimensUpdateDTO dto);
    Task<List<ARVRegimensReadOnlyDTO>> GetAllARVRegimensAsync();
    Task<ARVRegimensReadOnlyDTO> GetARVRegimensByIdAsync(int id);
    Task<bool> DeleteARVRegimensAsync(int id);
}