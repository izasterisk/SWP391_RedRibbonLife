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
    public FutureDateAttribute()
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;
            
        if (!(value is DateOnly || value is DateTime))
        {
            return new ValidationResult("Invalid date format. Please provide a valid date.");
        }
        
        if (value is DateOnly dateOnly)
        {
            if (dateOnly <= DateOnly.FromDateTime(DateTime.Now))
            {
                return new ValidationResult("Date must be in the future.");
            }
        }
        
        if (value is DateTime dateTime)
        {
            if (dateTime.Date <= DateTime.Now.Date)
            {
                return new ValidationResult("Date must be in the future.");
            }
        }
        
        return ValidationResult.Success;
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
            var morningStart = new TimeOnly(7, 0);
            var morningEnd = new TimeOnly(11, 0);
            var afternoonStart = new TimeOnly(13, 0);
            var afternoonEnd = new TimeOnly(17, 0);
            return (timeOnly >= morningStart && timeOnly <= morningEnd) ||
                   (timeOnly >= afternoonStart && timeOnly <= afternoonEnd);
        }
        return false;
    }
}

public class DateRangeAttribute : ValidationAttribute
{
    private readonly string _startDateProperty;
    
    public DateRangeAttribute(string startDateProperty) : base("End date must be after start date.")
    {
        _startDateProperty = startDateProperty;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var startDateProperty = validationContext.ObjectType.GetProperty(_startDateProperty);
        if (startDateProperty == null)
            return new ValidationResult($"Property {_startDateProperty} not found.");
        var startDateValue = startDateProperty.GetValue(validationContext.ObjectInstance);
        if (value is DateOnly endDate && startDateValue is DateOnly startDate)
        {
            if (endDate <= startDate)
                return new ValidationResult("End date must be after start date.");
        }
        else
        {
            return new ValidationResult("Invalid date format for date range validation.");
        }
        return ValidationResult.Success;
    }
} 