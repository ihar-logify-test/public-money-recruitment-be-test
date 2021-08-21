using System.Collections.Generic;
using System;
using VacationRental.DAL.Model;

namespace VacationRental.DAL.Repositories
{
    public interface IBookingRepository : IGenericRepository<int, BookingDataModel>
    {
        int BookedUnitsCount(int rentalId, DateTime start, DateTime end);
        IEnumerable<BookingDataModel> LoadBookingsForPeriod(int rentalId, DateTime start, DateTime end);
    }
}