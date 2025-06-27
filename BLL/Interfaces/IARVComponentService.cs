using BLL.DTO.ARVComponent;

namespace BLL.Interfaces;

public interface IARVComponentService
{
    Task<ARVComponentDTO> CreateARVComponentAsync(ARVComponentCreateDTO dto);
    Task<ARVComponentDTO> UpdateARVComponentAsync(ARVComponentUpdateDTO dto);
    Task<List<ARVComponentDTO>> GetAllARVComponentAsync();
    Task<ARVComponentDTO> GetARVComponentByIdAsync(int id);
    Task<bool> DeleteARVComponentAsync(int id);
}