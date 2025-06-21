using System.ComponentModel.DataAnnotations;

namespace BLL.Utils;

public class TimeValidatorAttribute : ValidationAttribute
{
    public TimeValidatorAttribute() : base("Appointment time must be a whole hour (minutes must be 0). For example: 8:00, 14:00, not 8:30 or 14:15")
    {
    }
    public override bool IsValid(object? value)
    {
        if (value == null)
            return true; // Let Required attribute handle null validation
        if (value is TimeOnly timeOnly)
        {
            return timeOnly.Minute == 0 && timeOnly.Second == 0 && timeOnly.Millisecond == 0;
        }
        return false;
    }
}

public class FutureDateAttribute : ValidationAttribute
{
    public FutureDateAttribute() : base("Appointment date must be in the future.")
    {
    }

    public override bool IsValid(object? value)
    {
        if (value == null)
            return true; // Let Required attribute handle null validation
            
        if (value is DateOnly dateOnly)
        {
            return dateOnly > DateOnly.FromDateTime(DateTime.Now);
        }
        
        if (value is DateTime dateTime)
        {
            return dateTime.Date > DateTime.Now.Date;
        }
        
        return false;
    }
}

public class WorkingHoursAttribute : ValidationAttribute
{
    public WorkingHoursAttribute() : base("Appointment time must be during working hours: 7:00 AM - 11:00 AM or 1:00 PM - 5:00 PM")
    {
    }

    public override bool IsValid(object? value)
    {
        if (value == null)
            return true; // Let Required attribute handle null validation
            
        if (value is TimeOnly timeOnly)
        {
            // Morning session: 7:00 AM - 11:00 AM
            var morningStart = new TimeOnly(7, 0);
            var morningEnd = new TimeOnly(11, 0);
            
            // Afternoon session: 1:00 PM - 5:00 PM  
            var afternoonStart = new TimeOnly(13, 0);
            var afternoonEnd = new TimeOnly(17, 0);
            
            return (timeOnly >= morningStart && timeOnly <= morningEnd) ||
                   (timeOnly >= afternoonStart && timeOnly <= afternoonEnd);
        }
        
        return false;
    }
} 