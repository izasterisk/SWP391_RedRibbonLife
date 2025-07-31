using DAL.IRepository;
using DAL.Models;

namespace DAL.Repository;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly IRepository<Appointment> _appointmentRepository;
    
    public AppointmentRepository(IRepository<Appointment> appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }
    
    public async Task<Appointment?> GetAppointmentToCheckAsync(int id, DateOnly date, TimeOnly time)
    {
        return await _appointmentRepository.GetAsync(
            u => u.DoctorId == id && u.AppointmentDate == date 
            && u.AppointmentTime == time 
            && (u.Status == "Confirmed" || u.Status == "Scheduled"), true);
    }
}