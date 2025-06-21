using AutoMapper;
using BLL.DTO.DoctorSchedule;
using BLL.Interfaces;
using DAL.IRepository;
using DAL.Models;

namespace BLL.Services;

public class DoctorScheduleService : IDoctorScheduleService
{
    private readonly IUserRepository<DoctorSchedule> _doctorScheduleRepository;
    private readonly IUserRepository<Doctor> _doctorRepository;
    private readonly IMapper _mapper;
    private readonly IUserUtils _userUtils;
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    private readonly IDoctorScheduleUtils _doctorScheduleUtils;
    public DoctorScheduleService(IUserRepository<DoctorSchedule> doctorScheduleRepository, IUserRepository<Doctor> doctorRepository, IMapper mapper, IUserUtils userUtils, SWP391_RedRibbonLifeContext dbContext, IDoctorScheduleUtils doctorScheduleUtils)
    {
        _doctorScheduleRepository = doctorScheduleRepository;
        _doctorRepository = doctorRepository;
        _mapper = mapper;
        _userUtils = userUtils;
        _dbContext = dbContext;
        _doctorScheduleUtils = doctorScheduleUtils;
    }
    
    public async Task<dynamic> CreateDoctorScheduleAsync(DoctorScheduleCreateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        _userUtils.CheckDoctorExist(dto.DoctorId);
        _userUtils.ValidateEndTimeStartTime(dto.StartTime, dto.EndTime);
        _doctorScheduleUtils.CheckDoctorScheduleExist(dto.DoctorId, dto.WorkDay);
        // Begin transaction
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            // Create doctor schedule
            DoctorSchedule doctorSchedule = _mapper.Map<DoctorSchedule>(dto);
            var createdDoctorSchedule = await _doctorScheduleRepository.CreateAsync(doctorSchedule);
            // Commit transaction
            await transaction.CommitAsync();
            return new
            {
                DoctorScheduleInfo = _mapper.Map<DoctorScheduleDTO>(createdDoctorSchedule)
            };
        }
        catch (Exception)
        {
            // Rollback transaction on error
            await transaction.RollbackAsync();
            throw; // Re-throw the exception
        }
    }
    
    public async Task<dynamic> UpdateDoctorScheduleAsync(DoctorScheduleUpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        // Get existing doctor schedule
        var doctorSchedule = await _doctorScheduleRepository.GetAsync(d => d.ScheduleId == dto.ScheduleId, true);
        if (doctorSchedule == null)
        {
            throw new Exception("Doctor schedule not found.");
        }
        _userUtils.ValidateEndTimeStartTime(dto.StartTime, dto.EndTime);
        // Update doctor schedule
        _mapper.Map(dto, doctorSchedule);
        var updatedDoctorSchedule = await _doctorScheduleRepository.UpdateAsync(doctorSchedule);
        return new
        {
            DoctorScheduleInfo = _mapper.Map<DoctorScheduleDTO>(updatedDoctorSchedule)
        };
    }
    
    public async Task<List<DoctorScheduleDTO>> GetDoctorScheduleByDoctorIdAsync(int id)
    {
        var doctorSchedule = await _doctorScheduleRepository.GetAllByFilterAsync(u => u.DoctorId == id, true);
        return _mapper.Map<List<DoctorScheduleDTO>>(doctorSchedule);
    }
    
    public async Task<bool> DeleteDoctorScheduleAsync(int id)
    {
        var doctorSchedule = await _doctorScheduleRepository.GetAsync(u => u.ScheduleId == id, true);
        if (doctorSchedule == null)
        {
            throw new Exception($"Doctor schedule with ID {id} not found");
        }
        await _doctorScheduleRepository.DeleteAsync(doctorSchedule);
        return true;
    }
}