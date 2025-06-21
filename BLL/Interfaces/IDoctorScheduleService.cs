using BLL.DTO.DoctorSchedule;

namespace BLL.Interfaces;

public interface IDoctorScheduleService
{
    Task<dynamic> CreateDoctorScheduleAsync(DoctorScheduleCreateDTO dto);
    Task<dynamic> UpdateDoctorScheduleAsync(DoctorScheduleUpdateDTO dto);
    Task<List<DoctorScheduleDTO>> GetDoctorScheduleByDoctorIdAsync(int id);
    Task<bool> DeleteDoctorScheduleAsync(int id);
}