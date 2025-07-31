using AutoMapper;
using BLL.DTO.Notification;
using BLL.Interfaces;
using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Utils;

public class NotificationUtils : INotificationUtils
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IMapper _mapper;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ITreatmentRepository _treatmentRepository;
    
    public NotificationUtils(INotificationRepository notificationRepository, IMapper mapper, IAppointmentRepository appointmentRepository, ITreatmentRepository treatmentRepository)
    {
        _notificationRepository = notificationRepository;
        _mapper = mapper;
        _appointmentRepository = appointmentRepository;
        _treatmentRepository = treatmentRepository;
    }
    
    public async Task<List<Appointment>> GetUpcomingAppointmentsAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var tomorrow = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
        var yesterday = DateTime.Now.Date.AddDays(-1);
        
        return await _notificationRepository.GetAllAppointmentsTodayNTomorrowAsync(today, tomorrow, yesterday);
    }

    public async Task<List<Treatment>> GetActiveTreatmentsForMedicationReminder(int frequency)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        return await _notificationRepository.GetAllActiveTreatmentsAsync(today, frequency);
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
        var notification = await _notificationRepository.GetById(dto.NotificationId);
        if (notification != null)
        {
            notification.ErrorMessage = dto.ErrorMessage;
            await _notificationRepository.UpdateAsync(notification);
        }
        return _mapper.Map<NotificationDTO>(notification);
    }
    
    public async Task MarkNotificationAsSentAsync(int notificationId)
    {
        var notification = await _notificationRepository.GetById(notificationId);
        if (notification != null)
        {
            notification.Status = "Sent";
            notification.SentAt = DateTime.Now;
            await _notificationRepository.UpdateAsync(notification);
        }
    }

    public async Task MarkNotificationAsFailedAsync(int notificationId, string errorMessage)
    {
        var notification = await _notificationRepository.GetById(notificationId);
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
        return await _notificationRepository.GetPendingNotificationsAsync();
    }

    public async Task<Appointment?> GetAppointmentByIdAsync(int appointmentId)
    {
        return await _appointmentRepository.GetAppointmentById4NotificationAsync(appointmentId);
    }

    public async Task<Treatment?> GetTreatmentByIdAsync(int treatmentId)
    {
        return await _treatmentRepository.GetTreatmentById4NotificationAsync(treatmentId);
    }
}