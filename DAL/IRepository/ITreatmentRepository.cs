using DAL.Models;

namespace DAL.IRepository;

public interface ITreatmentRepository
{
    Task<Treatment?> GetTreatmentToCheckAsync(int id);
    Task<Treatment?> GetTreatmentById4NotificationAsync(int id);
}