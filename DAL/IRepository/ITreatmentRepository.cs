using DAL.Models;

namespace DAL.IRepository;

public interface ITreatmentRepository
{
    Task<Treatment?> GetTreatmentToCheckAsync(int id);
    Task<Treatment?> GetTreatmentById4NotificationAsync(int id);
    
    // New methods for business operations
    Task<bool> CheckTreatmentExistsByTestResultIdAsync(int testResultId);
    Task<Treatment> CreateTreatmentWithTransactionAsync(Treatment treatment);
    Task<Treatment> UpdateTreatmentWithTransactionAsync(Treatment treatment);
    Task<List<Treatment>> GetAllTreatmentsWithDetailsAsync();
    Task<Treatment?> GetTreatmentWithDetailsByIdAsync(int treatmentId);
    Task<Treatment?> GetTreatmentWithDetailsByTestResultIdAsync(int testResultId);
    Task<List<Treatment>> GetTreatmentsByPatientIdAsync(int patientId);
    Task<Treatment?> GetTreatmentForUpdateAsync(int treatmentId);
    Task<bool> DeleteTreatmentWithTransactionAsync(Treatment treatment);
}