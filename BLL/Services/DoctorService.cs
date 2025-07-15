using AutoMapper;
using BLL.DTO.Doctor;
using BLL.Interfaces;
using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IMapper _mapper;
        private readonly SWP391_RedRibbonLifeContext _dbContext;
        private readonly IUserRepository<User> _userRepository;
        private readonly IUserRepository<Doctor> _doctorRepository;
        private readonly IUserUtils _userUtils;

        public DoctorService(IMapper mapper, SWP391_RedRibbonLifeContext dbContext, IUserRepository<User> userRepository, IUserRepository<Doctor> doctorRepository, IUserUtils userUtils)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _userRepository = userRepository;
            _doctorRepository = doctorRepository;
            _userUtils = userUtils;
        }

        public async Task<DoctorReadOnlyDTO> CreateDoctorAsync(DoctorCreateDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
            var usernameExists = await _userRepository.AnyAsync(u => u.Username.Equals(dto.Username));
            if (usernameExists)
            {
                throw new Exception($"Username {dto.Username} already exists.");
            }
            var emailExists = await _userRepository.AnyAsync(u => u.Email.Equals(dto.Email));
            if (emailExists)
            {
                throw new Exception($"Email {dto.Email} already exists.");
            }
            
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                User user = _mapper.Map<User>(dto);
                user.IsActive = true;
                user.UserRole = "Doctor";
                user.IsVerified = false;
                user.Password = _userUtils.CreatePasswordHash(dto.Password);
                var createdUser = await _userRepository.CreateAsync(user);
                Doctor doctor = new Doctor
                {
                    UserId = createdUser.UserId,
                    // DoctorImage = dto.DoctorImage,
                    Bio = dto.Bio
                };
                var createdDoctor = await _doctorRepository.CreateAsync(doctor);
                await transaction.CommitAsync();
                var detailedDoctor = await _doctorRepository.GetWithRelationsAsync(
                    filter: d => d.DoctorId == createdDoctor.DoctorId,
                    useNoTracking: true,
                    includeFunc: query => query.Include(d => d.User)
                );
                return _mapper.Map<DoctorReadOnlyDTO>(detailedDoctor);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        
        public async Task<DoctorReadOnlyDTO> UpdateDoctorAsync(DoctorUpdateDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var doctor = await _doctorRepository.GetAsync(d => d.DoctorId == dto.DoctorId, true);
                if (doctor == null)
                {
                    throw new Exception("Doctor not found.");
                }
                var user = await _userRepository.GetAsync(u => u.UserId == doctor.UserId, true);
                if (user == null)
                {
                    throw new Exception($"User associated with doctor ID {dto.DoctorId} not found.");
                }
                _mapper.Map(dto, user);
                _mapper.Map(dto, doctor);
                var updatedUser = await _userRepository.UpdateAsync(user);
                var updatedDoctor = await _doctorRepository.UpdateAsync(doctor);
                await transaction.CommitAsync();
                var detailedDoctor = await _doctorRepository.GetWithRelationsAsync(
                    filter: d => d.DoctorId == updatedDoctor.DoctorId,
                    useNoTracking: true,
                    includeFunc: query => query.Include(d => d.User)
                );
                return _mapper.Map<DoctorReadOnlyDTO>(detailedDoctor);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        
        public async Task<List<DoctorReadOnlyDTO>> GetAllDoctorsAsync()
        {
            var doctors = await _doctorRepository.GetAllWithRelationsAsync(
                includeFunc: query => query.Include(d => d.User)
                    .Where(d => d.User.IsActive && d.User.UserRole == "Doctor")
            );
            return _mapper.Map<List<DoctorReadOnlyDTO>>(doctors);
        }
        
        public async Task<DoctorReadOnlyDTO> GetDoctorByDoctorIDAsync(int id)
        {
            var doctor = await _doctorRepository.GetWithRelationsAsync(
                filter: d => d.DoctorId == id,
                useNoTracking: true,
                includeFunc: query => query.Include(d => d.User)
            );
            if (doctor == null)
            {
                throw new Exception("Doctor not found.");
            }
            return _mapper.Map<DoctorReadOnlyDTO>(doctor);
        }
        
        public async Task<bool> DeleteDoctorByDoctorIdAsync(int id)
        {
            var doctor = await _doctorRepository.GetAsync(d => d.DoctorId == id, true);
            if (doctor == null)
            {
                throw new Exception("Doctor not found.");
            }
            var user = await _userRepository.GetAsync(u => u.UserId == doctor.UserId, true);
            if (user == null)
            {
                throw new Exception($"User associated with Doctor ID {id} not found.");
            }
            await _doctorRepository.DeleteAsync(doctor);
            await _userRepository.DeleteAsync(user);
            return true;
        }
    }
}
