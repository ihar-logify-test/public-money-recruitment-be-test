using System;

using VacationRental.Contract.Models;

namespace VacationRental.BLL.Services.Interfaces
{
    public interface IRentalService
    {
        int CreateRental(RentalBindingModel rental);
        int CreateBooking(BookingBindingModel booking);
        RentalViewModel GetRental(int rentalId);
        BookingViewModel GetBooking(int bookingId);
        CalendarViewModel GetCalendar(GetCalendarModel getCalendarModel);
    }
}