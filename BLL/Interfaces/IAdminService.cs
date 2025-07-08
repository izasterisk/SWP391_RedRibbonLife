using BLL.DTO.Admin;

namespace BLL.Interfaces;

public interface IAdminService
{
    Task<AdminReadOnlyDTO> CreateAdminAsync(AdminDTO dto);
    Task<AdminReadOnlyDTO> UpdateAdminAsync(AdminUpdateDTO dto);
    Task<List<AdminReadOnlyDTO>> GetAllAdminsAsync();
    Task<AdminReadOnlyDTO> GetAdminByUserIdAsync(int id);
    Task<bool> DeleteAdminByIdAsync(int id);
}