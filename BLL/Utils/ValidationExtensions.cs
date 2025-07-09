using System;
using System.Threading.Tasks;

namespace BLL.Utils
{
    public static class ValidationExtensions
    {
        public static async Task ValidateIfNotNullAsync(this int? id, Func<int, Task> validator)
        {
            if (id.HasValue)
                await validator(id.Value);
        }
        
        public static async Task ValidateIfNotNullOrEmptyAsync(this string? value, Func<string, Task> validator)
        {
            if (!string.IsNullOrEmpty(value))
                await validator(value);
        }
        
        public static async Task ValidateIfNotNullAsync(this Guid? id, Func<Guid, Task> validator)
        {
            if (id.HasValue)
                await validator(id.Value);
        }
        
        public static async Task ValidateIfNotNullAsync(this DateTime? date, Func<DateTime, Task> validator)
        {
            if (date.HasValue)
                await validator(date.Value);
        }
    }
} 