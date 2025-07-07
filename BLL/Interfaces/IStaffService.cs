using BLL.DTO.Staff;

namespace BLL.Interfaces;

public interface IStaffService
{
    Task<StaffReadOnlyDTO> CreateStaffAsync(StaffDTO dto);
    Task<StaffReadOnlyDTO> UpdateStaffAsync(StaffUpdateDTO dto);
    Task<List<StaffReadOnlyDTO>> GetAllStaffsAsync();
    Task<StaffReadOnlyDTO> GetStaffByIdAsync(int id);
    Task<bool> DeleteStaffByIdAsync(int id);
}