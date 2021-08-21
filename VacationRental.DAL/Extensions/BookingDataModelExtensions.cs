using System;
using VacationRental.DAL.Model;

namespace VacationRental.DAL.Extensions
{
    public static class BookingDataModelExtensions
    {
        public static DateTime GetEndDate(this BookingDataModel booking)
        {
            return booking.Start.AddDays(booking.Nights);
        }
    }
}