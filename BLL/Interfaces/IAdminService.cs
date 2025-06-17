using BLL.DTO.Admin;

namespace BLL.Interfaces;

public interface IAdminService
{
    Task<dynamic> CreateAdminAsync(AdminDTO dto);
    Task<dynamic> UpdateAdminAsync(AdminUpdateDTO dto);
    Task<List<AdminReadOnlyDTO>> GetAllAdminsAsync();
    Task<AdminReadOnlyDTO> GetAdminByAdminIDAsync(int id);
    Task<bool> DeleteAdminAsync(int id);
}