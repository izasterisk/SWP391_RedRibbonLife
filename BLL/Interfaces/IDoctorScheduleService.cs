using BLL.DTO.DoctorSchedule;

namespace BLL.Interfaces;

public interface IDoctorScheduleService
{
    Task<DoctorScheduleDTO> CreateDoctorScheduleAsync(DoctorScheduleCreateDTO dto);
    Task<DoctorScheduleDTO> UpdateDoctorScheduleAsync(DoctorScheduleUpdateDTO dto);
    Task<List<DoctorScheduleDTO>> GetAllDoctorScheduleByDoctorIdAsync(int id);
    Task<DoctorScheduleDTO> GetDoctorScheduleByIdAsync(int id);
    Task<bool> DeleteDoctorScheduleByIdAsync(int id);
}