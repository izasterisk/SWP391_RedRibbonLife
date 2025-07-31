using DAL.Models;

namespace DAL.IRepository;

public interface IEmailRepository
{
    Task<User?> GetUnverifiedUserByEmailAsync(string email);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User> UpdateUserVerificationStatusAsync(User user, bool isVerified);
    Task<User> UpdateUserPasswordAsync(User user, string passwordHash);
    Task<Treatment?> GetTreatmentWithRelationsAsync(int treatmentId);
}