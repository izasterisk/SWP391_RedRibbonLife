using DAL.Models;

namespace DAL.IRepository;

public interface IDoctorScheduleRepository
{
    Task<DoctorSchedule?> GetDoctorScheduleToCheckAsync(int id, string day);
    Task<DoctorSchedule> CreateDoctorScheduleWithTransactionAsync(DoctorSchedule doctorSchedule);
    Task<DoctorSchedule> UpdateDoctorScheduleWithTransactionAsync(DoctorSchedule doctorSchedule);
    Task<DoctorSchedule?> GetDoctorScheduleWithRelationsAsync(int scheduleId, bool useNoTracking = true);
    Task<DoctorSchedule?> GetDoctorScheduleForUpdateAsync(int scheduleId);
    Task<List<DoctorSchedule>> GetAllDoctorSchedulesByDoctorIdAsync(int doctorId);
    Task<bool> DeleteDoctorScheduleWithTransactionAsync(DoctorSchedule doctorSchedule);
}