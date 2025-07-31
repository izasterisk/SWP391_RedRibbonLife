using DAL.IRepository;
using DAL.Models;

namespace DAL.Repository;

public class AuthRepository : IAuthRepository
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IRepository<Doctor> _doctorRepository;

    public AuthRepository(IRepository<User> userRepository, IRepository<Patient> patientRepository, IRepository<Doctor> doctorRepository)
    {
        _userRepository = userRepository;
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _userRepository.GetAsync(u => u.Username.Equals(username) && u.IsActive);
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _userRepository.GetAsync(u => u.UserId == userId && u.IsActive);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        string lowercaseEmail = email.ToLowerInvariant();
        return await _userRepository.GetAsync(u => u.Email.ToLower().Equals(lowercaseEmail) && u.IsActive);
    }

    public async Task<Patient?> GetPatientByUserIdAsync(int userId)
    {
        return await _patientRepository.GetAsync(p => p.UserId == userId);
    }

    public async Task<Doctor?> GetDoctorByUserIdAsync(int userId)
    {
        return await _doctorRepository.GetAsync(d => d.UserId == userId);
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        return await _userRepository.UpdateAsync(user);
    }
}