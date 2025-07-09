using AutoMapper;
using BLL.DTO.DoctorSchedule;
using BLL.Interfaces;
using BLL.Utils;
using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class DoctorScheduleService : IDoctorScheduleService
{
    private readonly IMapper _mapper;
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    private readonly IUserRepository<DoctorSchedule> _doctorScheduleRepository;
    private readonly IUserRepository<Doctor> _doctorRepository;
    private readonly IUserUtils _userUtils;
    private readonly IDoctorScheduleUtils _doctorScheduleUtils;
    
    public DoctorScheduleService(IMapper mapper, SWP391_RedRibbonLifeContext dbContext, IUserRepository<DoctorSchedule> doctorScheduleRepository, IUserRepository<Doctor> doctorRepository, IUserUtils userUtils, IDoctorScheduleUtils doctorScheduleUtils)
    {
        _mapper = mapper;
        _dbContext = dbContext;
        _doctorScheduleRepository = doctorScheduleRepository;
        _doctorRepository = doctorRepository;
        _userUtils = userUtils;
        _doctorScheduleUtils = doctorScheduleUtils;
    }
    
    public async Task<DoctorScheduleDTO> CreateDoctorScheduleAsync(DoctorScheduleCreateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        await _userUtils.CheckDoctorExistAsync(dto.DoctorId);
        _userUtils.ValidateEndTimeStartTime(dto.StartTime, dto.EndTime);
        await _doctorScheduleUtils.CheckDoctorScheduleExistAsync(dto.DoctorId, dto.WorkDay);
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            DoctorSchedule doctorSchedule = _mapper.Map<DoctorSchedule>(dto);
            var createdDoctorSchedule = await _doctorScheduleRepository.CreateAsync(doctorSchedule);
            await transaction.CommitAsync();
            var detailedDoctorSchedule = await _doctorScheduleRepository.GetWithRelationsAsync(
                filter: ds => ds.ScheduleId == createdDoctorSchedule.ScheduleId,
                useNoTracking: true,
                includeFunc: query => query
                    .Include(ds => ds.Doctor)
                        .ThenInclude(d => d.User)
            );
            return _mapper.Map<DoctorScheduleDTO>(detailedDoctorSchedule);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<DoctorScheduleDTO> UpdateDoctorScheduleAsync(DoctorScheduleUpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var doctorSchedule = await _doctorScheduleRepository.GetAsync(d => d.ScheduleId == dto.ScheduleId, true);
            if (doctorSchedule == null)
            {
                throw new Exception("Doctor schedule not found.");
            }
            var finalStartTime = dto.StartTime ?? doctorSchedule.StartTime;
            var finalEndTime = dto.EndTime ?? doctorSchedule.EndTime;
            _userUtils.ValidateEndTimeStartTime(finalStartTime, finalEndTime);
            _mapper.Map(dto, doctorSchedule);
            var updatedDoctorSchedule = await _doctorScheduleRepository.UpdateAsync(doctorSchedule);
            await transaction.CommitAsync();
            var detailedDoctorSchedule = await _doctorScheduleRepository.GetWithRelationsAsync(
                filter: ds => ds.ScheduleId == updatedDoctorSchedule.ScheduleId,
                useNoTracking: true,
                includeFunc: query => query
                    .Include(ds => ds.Doctor)
                        .ThenInclude(d => d.User)
            );
            return _mapper.Map<DoctorScheduleDTO>(detailedDoctorSchedule);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<List<DoctorScheduleDTO>> GetAllDoctorScheduleByDoctorIdAsync(int id)
    {
        var doctorSchedules = await _dbContext.DoctorSchedules
            .Where(ds => ds.DoctorId == id)
            .Include(ds => ds.Doctor)
                .ThenInclude(d => d.User)
            .AsNoTracking()
            .ToListAsync();
        return _mapper.Map<List<DoctorScheduleDTO>>(doctorSchedules);
    }
    
    public async Task<DoctorScheduleDTO> GetDoctorScheduleByIdAsync(int id)
    {
        var doctorSchedule = await _doctorScheduleRepository.GetWithRelationsAsync(
            filter: ds => ds.ScheduleId == id,
            useNoTracking: true,
            includeFunc: query => query
                .Include(ds => ds.Doctor)
                    .ThenInclude(d => d.User)
        );
        if (doctorSchedule == null)
        {
            throw new Exception("Doctor schedule not found.");
        }
        return _mapper.Map<DoctorScheduleDTO>(doctorSchedule);
    }
    
    public async Task<bool> DeleteDoctorScheduleByIdAsync(int id)
    {
        var doctorSchedule = await _doctorScheduleRepository.GetAsync(u => u.ScheduleId == id, true);
        if (doctorSchedule == null)
        {
            throw new Exception("Doctor schedule not found.");
        }
        await _doctorScheduleRepository.DeleteAsync(doctorSchedule);
        return true;
    }
}