using DAL.IRepository;
using DAL.Models;

namespace DAL.Repository;

public class DoctorScheduleRepository : IDoctorScheduleRepository
{
    private readonly IRepository<DoctorSchedule> _doctorScheduleRepository;
    
    public DoctorScheduleRepository(IRepository<DoctorSchedule> doctorScheduleRepository)
    {
        _doctorScheduleRepository = doctorScheduleRepository;
    }
    
    public async Task<DoctorSchedule?> GetDoctorScheduleToCheckAsync(int id, string day)
    {
        return await _doctorScheduleRepository.GetAsync(
            u => u.DoctorId == id && u.WorkDay == day, 
            true
        );
    }
}