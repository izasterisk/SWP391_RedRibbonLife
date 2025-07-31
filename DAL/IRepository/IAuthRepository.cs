using DAL.Models;

namespace DAL.IRepository;

public interface IAuthRepository
{
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User?> GetUserByIdAsync(int userId);
    Task<User?> GetUserByEmailAsync(string email);
    Task<Patient?> GetPatientByUserIdAsync(int userId);
    Task<Doctor?> GetDoctorByUserIdAsync(int userId);
    Task<User> UpdateUserAsync(User user);
}