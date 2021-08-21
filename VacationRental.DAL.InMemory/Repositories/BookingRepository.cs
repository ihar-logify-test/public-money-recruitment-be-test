using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using VacationRental.DAL.Model;
using VacationRental.DAL.Repositories;

namespace VacationRental.DAL.InMemory.Repositories
{
    public class BookingRepository : InMemoryRepository<BookingDataModel>, IBookingRepository
    {
        public int BookedUnitsCount(int rentalId, DateTime start, DateTime end)
        {
            return LoadBookingsForPeriodQuery(rentalId, start, end).Count();
        }
        
        public IEnumerable<BookingDataModel> LoadBookingsForPeriod(int rentalId, DateTime start, DateTime end)
        {
            return LoadBookingsForPeriodQuery(rentalId, start, end).AsEnumerable();
        }

        private IQueryable<BookingDataModel> LoadBookingsForPeriodQuery(int rentalId, DateTime start, DateTime end)
        {
            return BookingsByRentalQuery(rentalId).Where(IsOverlappingFilter(start, end));
        }


        private Expression<Func<BookingDataModel, bool>> IsOverlappingFilter(DateTime start, DateTime end)
        {
            var startDate = start.Date;
            var endDate = end.Date;
            return booking => (booking.Start <= startDate && booking.Start.AddDays(booking.Nights) > startDate)
                        || (booking.Start < endDate && booking.Start.AddDays(booking.Nights) >= endDate)
                        || (booking.Start > startDate && booking.Start.AddDays(booking.Nights) < endDate);
        }
        
        private IQueryable<BookingDataModel> BookingsByRentalQuery(int rentalId) => _storage.Values.AsQueryable().Where(_ => _.RentalId == rentalId);

    }
}