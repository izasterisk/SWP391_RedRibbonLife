using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository;

public class PatientRepository : IPatientRepository
{
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Patient> _patientRepository;

    public PatientRepository(SWP391_RedRibbonLifeContext dbContext, IRepository<User> userRepository, IRepository<Patient> patientRepository)
    {
        _dbContext = dbContext;
        _userRepository = userRepository;
        _patientRepository = patientRepository;
    }

    public async Task<bool> CheckUsernameExistsAsync(string username)
    {
        return await _userRepository.AnyAsync(u => u.Username.Equals(username));
    }

    public async Task<Patient> CreatePatientWithTransactionAsync(User user, Patient patient)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            user.Email = user.Email.ToLowerInvariant();
            var createdUser = await _userRepository.CreateAsync(user);
            patient.UserId = createdUser.UserId;
            var createdPatient = await _patientRepository.CreateAsync(patient);
            await transaction.CommitAsync();
            return createdPatient;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Patient> UpdatePatientWithTransactionAsync(User user, Patient patient)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var updatedUser = await _userRepository.UpdateAsync(user);
            var updatedPatient = await _patientRepository.UpdateAsync(patient);
            await transaction.CommitAsync();
            return updatedPatient;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<Patient>> GetAllActivePatientsWithUserAsync()
    {
        return await _patientRepository.GetAllWithRelationsAsync(
            includeFunc: query => query.Include(p => p.User)
                .Where(p => p.User.IsActive && p.User.UserRole == "Patient")
        );
    }

    public async Task<Patient?> GetPatientWithUserAsync(int patientId, bool useNoTracking = true)
    {
        return await _patientRepository.GetWithRelationsAsync(
            filter: p => p.PatientId == patientId,
            useNoTracking: useNoTracking,
            includeFunc: query => query.Include(p => p.User)
        );
    }

    public async Task<Patient?> GetPatientForUpdateAsync(int patientId)
    {
        return await _patientRepository.GetAsync(p => p.PatientId == patientId, false);
    }

    public async Task<User?> GetUserByUserIdAsync(int userId, bool useNoTracking = false)
    {
        return await _userRepository.GetAsync(u => u.UserId == userId, useNoTracking);
    }

    public async Task<bool> DeletePatientWithTransactionAsync(Patient patient, User user)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            await _patientRepository.DeleteAsync(patient);
            await _userRepository.DeleteAsync(user);
            await transaction.CommitAsync();
            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}