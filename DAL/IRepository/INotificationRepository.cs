using DAL.Models;
using System.Linq.Expressions;

namespace DAL.IRepository;

public interface INotificationRepository
{
    Task<List<Appointment>> GetAllAppointmentsTodayNTomorrowAsync(DateOnly today, DateOnly tomorrow, DateTime yesterday);
    Task<List<Treatment>> GetAllActiveTreatmentsAsync(DateOnly today, int frequency);
    Task<Notification> CreateAsync(Notification notification);
    Task<Notification> UpdateAsync(Notification notification);
    Task<Notification> GetById(int id);
    Task<List<Notification>> GetPendingNotificationsAsync();
}