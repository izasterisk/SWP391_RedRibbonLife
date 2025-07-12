using AutoMapper;
using BLL.DTO.Notification;
using BLL.Interfaces;
using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Utils;

public class NotificationUtils : INotificationUtils
{
    private readonly IUserRepository<Appointment> _appointmentRepository;
    private readonly IUserRepository<Treatment> _treatmentRepository;
    private readonly IUserRepository<Notification> _notificationRepository;
    private readonly IMapper _mapper;
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    private readonly IUserUtils _userUtils;
    public NotificationUtils(IUserRepository<Appointment> appointmentRepository, IUserRepository<Treatment> treatmentRepository, IUserRepository<Notification> notificationRepository, IMapper mapper, SWP391_RedRibbonLifeContext dbContext, IUserUtils userUtils)
    {
        _appointmentRepository = appointmentRepository;
        _treatmentRepository = treatmentRepository;
        _notificationRepository = notificationRepository;
        _mapper = mapper;
        _dbContext = dbContext;
        _userUtils = userUtils;
    }

    public async Task<List<Appointment>> GetUpcomingAppointmentsAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var tomorrow = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
        var yesterday = DateTime.Now.Date.AddDays(-1);
        // Lấy tất cả appointments trong hôm nay và ngày mai
        var todayTomorrowAppointments = await _appointmentRepository.GetAllWithRelationsAsync(query => query
            .Include(a => a.Patient)
                 .ThenInclude(p => p.User)
            .Include(a => a.Doctor)
                 .ThenInclude(d => d.User)
            .Where(a => (a.AppointmentDate == today || a.AppointmentDate == tomorrow) && (a.Status == "Confirmed")));
        // Lấy danh sách AppointmentId đã được gửi notification từ hôm qua đến hiện tại
        var sentNotificationAppointmentIds = await _notificationRepository.GetAllByFilterAsync(n => 
            n.NotificationType == "Appointment" && 
            n.AppointmentId.HasValue && 
            n.Status == "Sent" &&
            n.SentAt.HasValue && 
            n.SentAt >= yesterday);
        var sentAppointmentIds = sentNotificationAppointmentIds
            .Select(n => n.AppointmentId.Value)
            .ToHashSet(); 
        // Loại bỏ những appointment đã gửi notification
        var appointmentsToNotify = todayTomorrowAppointments
            .Where(a => !sentAppointmentIds.Contains(a.AppointmentId))
            .ToList();
        return appointmentsToNotify;
    }

    public async Task<List<Treatment>> GetActiveTreatmentsForMedicationReminder(int frequency)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        return await _treatmentRepository.GetAllWithRelationsAsync(query =>
            query.Include(t => t.Regimen)
                 .Include(t => t.TestResult)
                 .ThenInclude(tr => tr.Patient)
                 .ThenInclude(p => p.User)
                 .Where(t => t.Status == "Active" && t.Regimen != null && t.StartDate != null && t.EndDate != null &&
                            t.StartDate <= today && t.EndDate >= today &&
                            (frequency == 1 ? t.Regimen.Frequency == 1 : t.Regimen.Frequency == frequency)));
    }

    public async Task<NotificationDTO> CreateNotificationAsync(NotificationCreateDTO dto)
    {
        var notification = _mapper.Map<Notification>(dto);
        notification.Status = "Pending";
        notification.RetryCount = 0;
        var createdNotification = await _notificationRepository.CreateAsync(notification);
        return _mapper.Map<NotificationDTO>(createdNotification);
    }

    public async Task<NotificationDTO> UpdateErrorMessageAsync(NotificationUpdateDTO dto)
    {
        var notification = await _notificationRepository.GetAsync(n => n.NotificationId == dto.NotificationId);
        if (notification != null)
        {
            notification.ErrorMessage = dto.ErrorMessage;
            await _notificationRepository.UpdateAsync(notification);
        }
        return _mapper.Map<NotificationDTO>(notification);
    }
    
    public async Task MarkNotificationAsSentAsync(int notificationId)
    {
        var notification = await _notificationRepository.GetAsync(n => n.NotificationId == notificationId);
        if (notification != null)
        {
            notification.Status = "Sent";
            notification.SentAt = DateTime.Now;
            await _notificationRepository.UpdateAsync(notification);
        }
    }

    public async Task MarkNotificationAsFailedAsync(int notificationId, string errorMessage)
    {
        var notification = await _notificationRepository.GetAsync(n => n.NotificationId == notificationId);
        if (notification != null)
        {
            notification.Status = "Failed";
            notification.RetryCount = (notification.RetryCount ?? 0) + 1;
            notification.ErrorMessage = errorMessage;
            await _notificationRepository.UpdateAsync(notification);
        }
    }

    public async Task<List<Notification>> GetPendingNotificationsAsync()
    {
        return await _notificationRepository.GetAllByFilterAsync(n => 
            n.Status == "Pending" && 
            n.ScheduledTime <= DateTime.Now &&
            (n.RetryCount ?? 0) < 3);
    }

    public async Task<Appointment?> GetAppointmentByIdAsync(int appointmentId)
    {
        return await _appointmentRepository.GetWithRelationsAsync(
            a => a.AppointmentId == appointmentId,
            useNoTracking: true,
            includeFunc: query => query
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
        );
    }

    public async Task<Treatment?> GetTreatmentByIdAsync(int treatmentId)
    {
        return await _treatmentRepository.GetWithRelationsAsync(
            t => t.TreatmentId == treatmentId,
            useNoTracking: true,
            includeFunc: query => query
                .Include(t => t.Regimen)
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.Patient)
                        .ThenInclude(p => p.User)
        );
    }
}