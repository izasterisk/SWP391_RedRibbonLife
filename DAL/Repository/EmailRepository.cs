using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository;

public class EmailRepository : IEmailRepository
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Treatment> _treatmentRepository;

    public EmailRepository(IRepository<User> userRepository, IRepository<Treatment> treatmentRepository)
    {
        _userRepository = userRepository;
        _treatmentRepository = treatmentRepository;
    }

    public async Task<User?> GetUnverifiedUserByEmailAsync(string email)
    {
        var lowercaseEmail = email.ToLower();
        return await _userRepository.GetAsync(u => u.Email.Equals(lowercaseEmail) && !u.IsVerified);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var lowercaseEmail = email.ToLower();
        return await _userRepository.GetAsync(u => u.Email.Equals(lowercaseEmail));
    }

    public async Task<User> UpdateUserVerificationStatusAsync(User user, bool isVerified)
    {
        user.IsVerified = isVerified;
        return await _userRepository.UpdateAsync(user);
    }

    public async Task<User> UpdateUserPasswordAsync(User user, string passwordHash)
    {
        user.Password = passwordHash;
        return await _userRepository.UpdateAsync(user);
    }

    public async Task<Treatment?> GetTreatmentWithRelationsAsync(int treatmentId)
    {
        return await _treatmentRepository.GetWithRelationsAsync(
            filter: t => t.TreatmentId == treatmentId,
            useNoTracking: true,
            includeFunc: query => query
                .Include(t => t.Regimen)
                    .ThenInclude(tr => tr.Component1)
                .Include(t => t.Regimen)
                    .ThenInclude(tr => tr.Component2)
                .Include(t => t.Regimen)
                    .ThenInclude(tr => tr.Component3)
                .Include(t => t.Regimen)
                    .ThenInclude(tr => tr.Component4)
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.Patient)
                    .ThenInclude(p => p.User)
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.Doctor)
                    .ThenInclude(d => d.User)
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.Appointment)
                .Include(t => t.TestResult)
                    .ThenInclude(tr => tr.TestType)
        );
    }
}