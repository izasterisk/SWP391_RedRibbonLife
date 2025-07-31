using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository;

public class DoctorRepository : IDoctorRepository
{
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Doctor> _doctorRepository;

    public DoctorRepository(SWP391_RedRibbonLifeContext dbContext, IRepository<User> userRepository, IRepository<Doctor> doctorRepository)
    {
        _dbContext = dbContext;
        _userRepository = userRepository;
        _doctorRepository = doctorRepository;
    }

    public async Task<bool> CheckUsernameExistsAsync(string username)
    {
        return await _userRepository.AnyAsync(u => u.Username.Equals(username));
    }

    public async Task<Doctor> CreateDoctorWithTransactionAsync(User user, Doctor doctor)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            user.Email = user.Email.ToLowerInvariant();
            var createdUser = await _userRepository.CreateAsync(user);
            doctor.UserId = createdUser.UserId;
            var createdDoctor = await _doctorRepository.CreateAsync(doctor);
            await transaction.CommitAsync();
            return createdDoctor;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Doctor> UpdateDoctorWithTransactionAsync(User user, Doctor doctor)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var updatedUser = await _userRepository.UpdateAsync(user);
            var updatedDoctor = await _doctorRepository.UpdateAsync(doctor);
            await transaction.CommitAsync();
            return updatedDoctor;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<Doctor>> GetAllDoctorsWithUserAsync()
    {
        return await _doctorRepository.GetAllWithRelationsAsync(
            includeFunc: query => query.Include(d => d.User)
                .Where(d => d.User.IsActive && d.User.UserRole == "Doctor")
        );
    }

    public async Task<Doctor?> GetDoctorWithUserAsync(int doctorId, bool useNoTracking = true)
    {
        return await _doctorRepository.GetWithRelationsAsync(
            filter: d => d.DoctorId == doctorId,
            useNoTracking: useNoTracking,
            includeFunc: query => query.Include(d => d.User)
        );
    }

    public async Task<Doctor?> GetDoctorForUpdateAsync(int doctorId)
    {
        return await _doctorRepository.GetAsync(d => d.DoctorId == doctorId, false);
    }

    public async Task<User?> GetUserByUserIdAsync(int userId, bool useNoTracking = false)
    {
        return await _userRepository.GetAsync(u => u.UserId == userId, useNoTracking);
    }

    public async Task<bool> DeleteDoctorWithTransactionAsync(Doctor doctor, User user)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            await _doctorRepository.DeleteAsync(doctor);
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