using System;
using System.Collections.Generic;
using System.Linq;

using VacationRental.DAL.Model;

namespace VacationRental.DAL.Extensions
{
    public static class BookingDataModelExtensions
    {
        public static DateTime GetEndDate(this BookingDataModel booking)
        {
            return booking.Start.AddDays(booking.Nights);
        }

        public static bool IsDateInBooking(this BookingDataModel booking, DateTime date)
        {
            return date >= booking.Start && date < GetEndDate(booking);
        }

        public static IEnumerable<DateTime> GetDatesList(this BookingDataModel booking, bool includeEndDate = false, int preparationDays = 0)
        {
            var startIndex = includeEndDate ? 1 : 0;
            return Enumerable.Range(startIndex, booking.Nights + preparationDays).Select(_ => booking.Start.AddDays(_));
        }

        public static DateTime GetPreparationStartDate(this BookingDataModel booking)
        {
            return booking.GetEndDate();
        }

        public static DateTime GetPreparationEndDate(this BookingDataModel booking, int preparationDays)
        {
            return booking.GetPreparationStartDate().AddDays(preparationDays);
        }
    }
}