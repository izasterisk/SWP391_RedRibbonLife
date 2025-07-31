using AutoMapper;
using BLL.Interfaces;
using DAL.IRepository;
using DAL.Models;
using System;
using System.Threading.Tasks;

namespace BLL.Utils;

public class DoctorScheduleUtils : IDoctorScheduleUtils
{
    private readonly IDoctorScheduleRepository _doctorScheduleRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    
    public DoctorScheduleUtils(IDoctorScheduleRepository doctorScheduleRepository, IAppointmentRepository appointmentRepository)
    {
        _doctorScheduleRepository = doctorScheduleRepository;
        _appointmentRepository = appointmentRepository;
    }
    public async Task CheckDoctorScheduleExistAsync(int id, string day)
    {
        if (!Enum.TryParse<DayOfWeek>(day, true, out _))
        {
            throw new ArgumentException("Invalid day of the week.");
        }
        day = char.ToUpper(day[0]) + day.Substring(1).ToLower();
        var doctorSchedule = await _doctorScheduleRepository.GetDoctorScheduleToCheckAsync(id, day);
        if (doctorSchedule != null)
        {
            throw new Exception("Doctor schedule already exists on this day.");
        }
    }

    public async Task CheckDoctorIfAvailableAsync(int id, DateOnly date, TimeOnly time)
    {
        string dayOfWeek = date.DayOfWeek.ToString();
        var doctorSchedule = await _doctorScheduleRepository.GetDoctorScheduleToCheckAsync(id, dayOfWeek);
        if (doctorSchedule == null)
        {
            throw new Exception("Doctor is not going to work on this day.");
        }
        if (time < doctorSchedule.StartTime || time > doctorSchedule.EndTime)
        {
            throw new Exception("Doctor is not available at this time.");
        }
        var appointment = await _appointmentRepository.GetAppointmentToCheckAsync(id, date, time);
        if (appointment != null)
        {
            throw new Exception("This doctor is already booked at this time.");
        }
    }
}