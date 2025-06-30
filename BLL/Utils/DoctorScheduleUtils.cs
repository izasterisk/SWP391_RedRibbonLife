using AutoMapper;
using BLL.Interfaces;
using DAL.IRepository;
using DAL.Models;

namespace BLL.Utils;

public class DoctorScheduleUtils : IDoctorScheduleUtils
{
    private readonly IUserRepository<DoctorSchedule> _doctorScheduleRepository;
    private readonly IUserRepository<Doctor> _doctorRepository;
    private readonly IUserRepository<Appointment> _appointmentRepository;
    private readonly IMapper _mapper;
    private readonly IUserUtils _userUtils;
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    public DoctorScheduleUtils(IUserRepository<DoctorSchedule> doctorScheduleRepository, IUserRepository<Doctor> doctorRepository, IUserRepository<Appointment> appointmentRepository, IMapper mapper, IUserUtils userUtils, SWP391_RedRibbonLifeContext dbContext)
    {
        _doctorScheduleRepository = doctorScheduleRepository;
        _doctorRepository = doctorRepository;
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
        _userUtils = userUtils;
        _dbContext = dbContext;
    }
    
    public void CheckDoctorScheduleExist(int id, string day)
    {
        if (!Enum.TryParse<DayOfWeek>(day, true, out _))
        {
            throw new ArgumentException("Invalid day of the week.");
        }
        day = char.ToUpper(day[0]) + day.Substring(1).ToLower();
        var doctorSchedule = _doctorScheduleRepository.GetAsync(
            u => u.DoctorId == id && u.WorkDay == day, 
            true
        ).GetAwaiter().GetResult();
        if (doctorSchedule != null)
        {
            throw new Exception("Doctor schedule already exists on this day.");
        }
    }

    public void CheckDoctorIfAvailable(int id, DateOnly date, TimeOnly time)
    {
        string dayOfWeek = date.DayOfWeek.ToString();
        var doctorSchedule = _doctorScheduleRepository.GetAsync(
            u => u.DoctorId == id && u.WorkDay == dayOfWeek, true).GetAwaiter().GetResult();
        if (doctorSchedule == null)
        {
            throw new Exception("Doctor is not going to work on this day.");
        }
        if (time < doctorSchedule.StartTime || time > doctorSchedule.EndTime)
        {
            throw new Exception("Doctor is not available at this time.");
        }
        var appointment = _appointmentRepository.GetAsync(
            u => u.DoctorId == id && u.AppointmentDate == date && u.AppointmentTime == time, true).GetAwaiter().GetResult();
        if (appointment != null)
        {
            throw new Exception("This doctor is already booked at this time.");
        }
    }
}