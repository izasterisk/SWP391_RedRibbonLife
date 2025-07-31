using DAL.Models;

namespace DAL.IRepository;

public interface IDoctorRepository
{
    Task<bool> CheckUsernameExistsAsync(string username);
    Task<Doctor> CreateDoctorWithTransactionAsync(User user, Doctor doctor);
    Task<Doctor> UpdateDoctorWithTransactionAsync(User user, Doctor doctor);
    Task<List<Doctor>> GetAllDoctorsWithUserAsync();
    Task<Doctor?> GetDoctorWithUserAsync(int doctorId, bool useNoTracking = true);
    Task<Doctor?> GetDoctorForUpdateAsync(int doctorId);
    Task<User?> GetUserByUserIdAsync(int userId, bool useNoTracking = false);
    Task<bool> DeleteDoctorWithTransactionAsync(Doctor doctor, User user);
}