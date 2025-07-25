using AutoMapper;
using BLL.DTO.AdminDashboard;
using BLL.Interfaces;
using BLL.Utils;
using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class AdminDashboardService : IAdminDashboardService
{
    private readonly IMapper _mapper;
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    private readonly IUserRepository<Appointment> _appointmentRepository;
    private readonly IUserRepository<Patient> _patientRepository;
    private readonly IUserRepository<Doctor> _doctorRepository;
    private readonly IUserRepository<DoctorSchedule> _doctorScheduleRepository;
    private readonly IUserUtils _userUtils;
    private readonly IDoctorScheduleUtils _doctorScheduleUtils;
    private readonly SendGridEmailUtil _sendGridUtil;
    
    public AdminDashboardService(IMapper mapper, SWP391_RedRibbonLifeContext dbContext, IUserRepository<Appointment> appointmentRepository, IUserRepository<Patient> patientRepository, IUserRepository<Doctor> doctorRepository, IUserRepository<DoctorSchedule> doctorScheduleRepository, IUserUtils userUtils, IDoctorScheduleUtils doctorScheduleUtils, SendGridEmailUtil sendGridUtil)
    {
        _mapper = mapper;
        _dbContext = dbContext;
        _appointmentRepository = appointmentRepository;
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
        _doctorScheduleRepository = doctorScheduleRepository;
        _userUtils = userUtils;
        _doctorScheduleUtils = doctorScheduleUtils;
        _sendGridUtil = sendGridUtil;
    }
    
    public async Task<List<AdminDashboardAppointmentDTO>> GetNumberOfAppointmentInTheLast6MonthsAsync()
    {
        var currentDate = DateTime.Now;
        var sixMonthsAgo = currentDate.AddMonths(-5);
        var startDate = new DateTime(sixMonthsAgo.Year, sixMonthsAgo.Month, 1);
        var endDate = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
        
        var appointmentCounts = await _appointmentRepository.GetAllWithRelationsAsync(
            includeFunc: query => query
                .Where(a => a.AppointmentDate >= DateOnly.FromDateTime(startDate) && 
                           a.AppointmentDate <= DateOnly.FromDateTime(endDate))
        );
        
        var groupedData = appointmentCounts
            .GroupBy(a => new { Year = a.AppointmentDate.Year, Month = a.AppointmentDate.Month })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Count = g.Count()
            })
            .ToList();
        
        var result = new List<AdminDashboardAppointmentDTO>();
        for (int i = 5; i >= 0; i--)
        {
            var targetDate = currentDate.AddMonths(-i);
            var existingData = groupedData.FirstOrDefault(x => x.Year == targetDate.Year && x.Month == targetDate.Month);
            
            result.Add(new AdminDashboardAppointmentDTO
            {
                month = targetDate.Month,
                count = existingData?.Count ?? 0
            });
        }
        
        return result;
    }
    
    
}