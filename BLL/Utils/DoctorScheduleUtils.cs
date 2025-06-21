using AutoMapper;
using BLL.Interfaces;
using DAL.IRepository;
using DAL.Models;

namespace BLL.Utils;

public class DoctorScheduleUtils : IDoctorScheduleUtils
{
    private readonly IUserRepository<DoctorSchedule> _doctorScheduleRepository;
    private readonly IUserRepository<Doctor> _doctorRepository;
    private readonly IMapper _mapper;
    private readonly IUserUtils _userUtils;
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    public DoctorScheduleUtils(IUserRepository<DoctorSchedule> doctorScheduleRepository, IUserRepository<Doctor> doctorRepository, IMapper mapper, IUserUtils userUtils, SWP391_RedRibbonLifeContext dbContext)
    {
        _doctorScheduleRepository = doctorScheduleRepository;
        _doctorRepository = doctorRepository;
        _mapper = mapper;
        _userUtils = userUtils;
        _dbContext = dbContext;
    }
    
    public void CheckDoctorScheduleExist(int id, string day)
    {
        // Kiểm tra xem day có phải là ngày trong tuần hợp lệ không
        if (!Enum.TryParse<DayOfWeek>(day, true, out _))
        {
            throw new ArgumentException("Invalid day of the week.");
        }
        // Chuẩn hóa format (chữ cái đầu viết hoa)
        day = char.ToUpper(day[0]) + day.Substring(1).ToLower();
        // Check if doctor schedule exists with both DoctorId and WorkDay
        var doctorSchedule = _doctorScheduleRepository.GetAsync(
            u => u.DoctorId == id && u.WorkDay == day, 
            true
        ).GetAwaiter().GetResult();
        if (doctorSchedule != null)
        {
            throw new Exception("Doctor schedule already exists on this day.");
        }
    }
    
    
}