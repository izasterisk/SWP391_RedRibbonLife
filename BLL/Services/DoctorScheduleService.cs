using AutoMapper;
using BLL.DTO.DoctorSchedule;
using BLL.Interfaces;
using BLL.Utils;
using DAL.IRepository;
using DAL.Models;

namespace BLL.Services;

public class DoctorScheduleService : IDoctorScheduleService
{
    private readonly IMapper _mapper;
    private readonly IDoctorScheduleRepository _doctorScheduleRepository;
    private readonly IUserUtils _userUtils;
    private readonly IDoctorScheduleUtils _doctorScheduleUtils;
    
    public DoctorScheduleService(IMapper mapper, IDoctorScheduleRepository doctorScheduleRepository, IUserUtils userUtils, IDoctorScheduleUtils doctorScheduleUtils)
    {
        _mapper = mapper;
        _doctorScheduleRepository = doctorScheduleRepository;
        _userUtils = userUtils;
        _doctorScheduleUtils = doctorScheduleUtils;
    }
    
    public async Task<DoctorScheduleDTO> CreateDoctorScheduleAsync(DoctorScheduleCreateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        await _userUtils.CheckDoctorExistAsync(dto.DoctorId);
        _userUtils.ValidateEndTimeStartTime(dto.StartTime, dto.EndTime);
        await _doctorScheduleUtils.CheckDoctorScheduleExistAsync(dto.DoctorId, dto.WorkDay);
        
        DoctorSchedule doctorSchedule = _mapper.Map<DoctorSchedule>(dto);
        var createdDoctorSchedule = await _doctorScheduleRepository.CreateDoctorScheduleWithTransactionAsync(doctorSchedule);
        var detailedDoctorSchedule = await _doctorScheduleRepository.GetDoctorScheduleWithRelationsAsync(createdDoctorSchedule.ScheduleId, true);
        return _mapper.Map<DoctorScheduleDTO>(detailedDoctorSchedule);
    }
    
    public async Task<DoctorScheduleDTO> UpdateDoctorScheduleAsync(DoctorScheduleUpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        
        var doctorSchedule = await _doctorScheduleRepository.GetDoctorScheduleForUpdateAsync(dto.ScheduleId);
        if (doctorSchedule == null)
        {
            throw new Exception("Doctor schedule not found.");
        }
        var finalStartTime = dto.StartTime ?? doctorSchedule.StartTime;
        var finalEndTime = dto.EndTime ?? doctorSchedule.EndTime;
        _userUtils.ValidateEndTimeStartTime(finalStartTime, finalEndTime);
        _mapper.Map(dto, doctorSchedule);
        var updatedDoctorSchedule = await _doctorScheduleRepository.UpdateDoctorScheduleWithTransactionAsync(doctorSchedule);
        var detailedDoctorSchedule = await _doctorScheduleRepository.GetDoctorScheduleWithRelationsAsync(updatedDoctorSchedule.ScheduleId, true);
        return _mapper.Map<DoctorScheduleDTO>(detailedDoctorSchedule);
    }
    
    public async Task<List<DoctorScheduleDTO>> GetAllDoctorScheduleByDoctorIdAsync(int id)
    {
        var doctorSchedules = await _doctorScheduleRepository.GetAllDoctorSchedulesByDoctorIdAsync(id);
        return _mapper.Map<List<DoctorScheduleDTO>>(doctorSchedules);
    }
    
    public async Task<DoctorScheduleDTO> GetDoctorScheduleByIdAsync(int id)
    {
        var doctorSchedule = await _doctorScheduleRepository.GetDoctorScheduleWithRelationsAsync(id, true);
        if (doctorSchedule == null)
        {
            throw new Exception("Doctor schedule not found.");
        }
        return _mapper.Map<DoctorScheduleDTO>(doctorSchedule);
    }
    
    public async Task<bool> DeleteDoctorScheduleByIdAsync(int id)
    {
        var doctorSchedule = await _doctorScheduleRepository.GetDoctorScheduleForUpdateAsync(id);
        if (doctorSchedule == null)
        {
            throw new Exception("Doctor schedule not found.");
        }
        return await _doctorScheduleRepository.DeleteDoctorScheduleWithTransactionAsync(doctorSchedule);
    }
}