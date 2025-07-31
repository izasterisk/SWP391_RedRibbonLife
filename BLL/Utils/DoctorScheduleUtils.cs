using AutoMapper;
using BLL.Interfaces;
using DAL.IRepository;
using DAL.Models;
using System;
using System.Threading.Tasks;

namespace BLL.Utils;

public class DoctorScheduleUtils : IDoctorScheduleUtils
{
    private readonly IRepository<DoctorSchedule> _doctorScheduleRepository;
    private readonly IRepository<Doctor> _doctorRepository;
    private readonly IRepository<Appointment> _appointmentRepository;
    private readonly IMapper _mapper;
    private readonly IUserUtils _userUtils;
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    public DoctorScheduleUtils(IRepository<DoctorSchedule> doctorScheduleRepository, IRepository<Doctor> doctorRepository, IRepository<Appointment> appointmentRepository, IMapper mapper, IUserUtils userUtils, SWP391_RedRibbonLifeContext dbContext)
    {
        _doctorScheduleRepository = doctorScheduleRepository;
        _doctorRepository = doctorRepository;
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
        _userUtils = userUtils;
        _dbContext = dbContext;
    }
    
    public async Task CheckDoctorScheduleExistAsync(int id, string day)
    {
        if (!Enum.TryParse<DayOfWeek>(day, true, out _))
        {
            throw new ArgumentException("Invalid day of the week.");
        }
        day = char.ToUpper(day[0]) + day.Substring(1).ToLower();
        var doctorSchedule = await _doctorScheduleRepository.GetAsync(
            u => u.DoctorId == id && u.WorkDay == day, 
            true
        );
        if (doctorSchedule != null)
        {
            throw new Exception("Doctor schedule already exists on this day.");
        }
    }

    public async Task CheckDoctorIfAvailableAsync(int id, DateOnly date, TimeOnly time)
    {
        string dayOfWeek = date.DayOfWeek.ToString();
        var doctorSchedule = await _doctorScheduleRepository.GetAsync(
            u => u.DoctorId == id && u.WorkDay == dayOfWeek, true);
        if (doctorSchedule == null)
        {
            throw new Exception("Doctor is not going to work on this day.");
        }
        if (time < doctorSchedule.StartTime || time > doctorSchedule.EndTime)
        {
            throw new Exception("Doctor is not available at this time.");
        }
        var appointment = await _appointmentRepository.GetAsync(
            u => u.DoctorId == id && u.AppointmentDate == date && u.AppointmentTime == time && (u.Status == "Confirmed" || u.Status == "Scheduled"), true);
        if (appointment != null)
        {
            throw new Exception("This doctor is already booked at this time.");
        }
    }
}