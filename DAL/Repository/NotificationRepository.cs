using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DAL.Repository;

public class NotificationRepository : INotificationRepository
{
    private readonly IRepository<Appointment> _appointmentRepository;
    private readonly IRepository<Treatment> _treatmentRepository;
    private readonly IRepository<Notification> _notificationRepository;
    
    public NotificationRepository(IRepository<Appointment> appointmentRepository, IRepository<Treatment> treatmentRepository, IRepository<Notification> notificationRepository)
    {
        _appointmentRepository = appointmentRepository;
        _treatmentRepository = treatmentRepository;
        _notificationRepository = notificationRepository;
    }
    
    public async Task<List<Appointment>> GetAllAppointmentsTodayNTomorrowAsync(DateOnly today, DateOnly tomorrow, DateTime yesterday)
    {
        // Lấy tất cả appointments trong hôm nay và ngày mai
        var todayTomorrowAppointments = await _appointmentRepository.GetAllWithRelationsAsync(query => query
            .Include(a => a.Patient)
                 .ThenInclude(p => p!.User)
            .Include(a => a.Doctor)
                 .ThenInclude(d => d!.User)
            .Where(a => (a.AppointmentDate == today || a.AppointmentDate == tomorrow) && (a.Status == "Confirmed")));
        
        // Lấy danh sách AppointmentId đã được gửi notification từ hôm qua đến hiện tại
        var sentNotificationAppointmentIds = await _notificationRepository.GetAllByFilterAsync(n => 
            n.NotificationType == "Appointment" && 
            n.AppointmentId.HasValue && 
            n.Status == "Sent" &&
            n.SentAt.HasValue && 
            n.SentAt >= yesterday);
        
        var sentAppointmentIds = sentNotificationAppointmentIds
            .Select(n => n.AppointmentId!.Value)
            .ToHashSet(); 
        
        // Loại bỏ những appointment đã gửi notification
        var appointmentsToNotify = todayTomorrowAppointments
            .Where(a => !sentAppointmentIds.Contains(a.AppointmentId))
            .ToList();
        
        return appointmentsToNotify;
    }

    public async Task<List<Treatment>> GetAllActiveTreatmentsAsync(DateOnly today, int frequency)
    {
        return await _treatmentRepository.GetAllWithRelationsAsync(query =>
            query.Include(t => t.Regimen)
                 .Include(t => t.TestResult)
                 .ThenInclude(tr => tr!.Patient)
                 .ThenInclude(p => p!.User)
                 .Where(t => t.Status == "Active" && t.Regimen != null && t.StartDate != null && t.EndDate != null &&
                            t.StartDate <= today && t.EndDate >= today &&
                            (frequency == 1 ? t.Regimen.Frequency == 1 : t.Regimen.Frequency == frequency)));
    }

    public async Task<Notification> CreateAsync(Notification notification)
    {
        return await _notificationRepository.CreateAsync(notification);
    }

    public async Task<Notification> UpdateAsync(Notification notification)
    {
        return await _notificationRepository.UpdateAsync(notification);
    }

    public async Task<Notification> GetById(int id)
    {
        return await _notificationRepository.GetAsync(n => n.NotificationId == id);
    }

    public async Task<List<Notification>> GetPendingNotificationsAsync()
    {
        return await _notificationRepository.GetAllByFilterAsync(n => 
            n.Status == "Pending" && 
            n.ScheduledTime <= DateTime.Now &&
            (n.RetryCount ?? 0) < 3);
    }
}