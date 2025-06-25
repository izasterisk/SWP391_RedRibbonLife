using System;

namespace BLL.Utils
{
    public static class ValidationExtensions
    {
        public static void ValidateIfNotNull(this int? id, Action<int> validator)
        {
            if (id.HasValue)
                validator(id.Value);
        }
        public static void ValidateIfNotNullOrEmpty(this string? value, Action<string> validator)
        {
            if (!string.IsNullOrEmpty(value))
                validator(value);
        }
        public static void ValidateIfNotNull(this Guid? id, Action<Guid> validator)
        {
            if (id.HasValue)
                validator(id.Value);
        }
        public static void ValidateIfNotNull(this DateTime? date, Action<DateTime> validator)
        {
            if (date.HasValue)
                validator(date.Value);
        }
    }
} 