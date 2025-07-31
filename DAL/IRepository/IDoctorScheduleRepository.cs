using DAL.Models;

namespace DAL.IRepository;

public interface IDoctorScheduleRepository
{
    Task<DoctorSchedule?> GetDoctorScheduleToCheckAsync(int id, string day);
}