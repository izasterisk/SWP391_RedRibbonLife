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