using System;

namespace VacationRental.BLL.Extensions
{
    public static class DateTimeExtensions
    {
        public static bool IsInRange(this DateTime targetDate, DateTime start, DateTime end)
        {
            return targetDate >= start && targetDate <= end;
        }
    }
}