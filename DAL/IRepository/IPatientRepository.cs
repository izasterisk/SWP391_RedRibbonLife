using DAL.Models;

namespace DAL.IRepository;

public interface IPatientRepository
{
    Task<bool> CheckUsernameExistsAsync(string username);
    Task<Patient> CreatePatientWithTransactionAsync(User user, Patient patient);
    Task<Patient> UpdatePatientWithTransactionAsync(User user, Patient patient);
    Task<List<Patient>> GetAllActivePatientsWithUserAsync();
    Task<Patient?> GetPatientWithUserAsync(int patientId, bool useNoTracking = true);
    Task<Patient?> GetPatientForUpdateAsync(int patientId);
    Task<User?> GetUserByUserIdAsync(int userId, bool useNoTracking = false);
    Task<bool> DeletePatientWithTransactionAsync(Patient patient, User user);
}