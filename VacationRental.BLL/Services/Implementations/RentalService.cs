using System;
using System.Collections.Generic;
using System.Linq;
using VacationRental.BLL.Services.Interfaces;
using VacationRental.Contract.Models;
using VacationRental.DAL.Model;
using VacationRental.DAL.Repositories;

namespace VacationRental.BLL.Services.Implementations
{
    public class RentalService : IRentalService
    {
        private readonly IRentalRepository _rentalRepository;
        private readonly IBookingRepository _bookingRepository;

        public RentalService(
            IBookingRepository bookingRepository, 
            IRentalRepository rentalRepository)
        {
            _bookingRepository = bookingRepository;
            _rentalRepository = rentalRepository;
        }

        public int CreateBooking(BookingBindingModel rentalModel)
        {
            if (rentalModel.Nights <= 0)
                throw new ApplicationException("Nigts must be positive");
            if (!_rentalRepository.LoadAll().Any(_ => _.Id == rentalModel.RentalId))
                throw new ApplicationException("Rental not found");

            for (var i = 0; i < rentalModel.Nights; i++)
            {
                var count = 0;
                foreach (var booking in _bookingRepository.LoadAll())
                {
                    if (booking.RentalId == rentalModel.RentalId
                        && (booking.Start <= rentalModel.Start.Date && booking.Start.AddDays(booking.Nights) > rentalModel.Start.Date)
                        || (booking.Start < rentalModel.Start.AddDays(rentalModel.Nights) && booking.Start.AddDays(booking.Nights) >= rentalModel.Start.AddDays(rentalModel.Nights))
                        || (booking.Start > rentalModel.Start && booking.Start.AddDays(booking.Nights) < rentalModel.Start.AddDays(rentalModel.Nights)))
                    {
                        count++;
                    }
                }
                if (count >= _rentalRepository.Load(rentalModel.RentalId).Units)
                    throw new ApplicationException("Not available");
            }
            
            var bookingToAdd = new Booking {Nights = rentalModel.Nights, Start = rentalModel.Start, RentalId = rentalModel.RentalId };
            return _bookingRepository.Add(bookingToAdd); 
        }

        public int CreateRental(RentalBindingModel rentalModel)
        {
            var rental = new Rental { Units = rentalModel.Units };
            return _rentalRepository.Add(rental);
        }

        public BookingViewModel GetBooking(int bookingId)
        {
            var booking = _bookingRepository.Load(bookingId);
            return new BookingViewModel {Id = booking.Id, Nights = booking.Nights, Start = booking.Start, RentalId = booking.RentalId };
        }

        public CalendarViewModel GetCalendar(int rentalId, DateTime start, int nights)
        {
            if (nights < 0)
                throw new ApplicationException("Nights must be positive");
            if (!_rentalRepository.LoadAll().Any(_ => _.Id == rentalId))
                throw new ApplicationException("Rental not found");

            var result = new CalendarViewModel 
            {
                RentalId = rentalId,
                Dates = new List<CalendarDateViewModel>() 
            };
            for (var i = 0; i < nights; i++)
            {
                var date = new CalendarDateViewModel
                {
                    Date = start.Date.AddDays(i),
                    Bookings = new List<CalendarBookingViewModel>()
                };

                foreach (var booking in _bookingRepository.LoadAll())
                {
                    if (booking.RentalId == rentalId
                        && booking.Start <= date.Date && booking.Start.AddDays(booking.Nights) > date.Date)
                    {
                        date.Bookings.Add(new CalendarBookingViewModel { Id = booking.Id });
                    }
                }

                result.Dates.Add(date);
            }

            return result;
        }

        public RentalViewModel GetRental(int rentalId)
        {
            var rental = _rentalRepository.Load(rentalId);
            return new RentalViewModel {Id = rental.Id, Units = rental.Units};
        }
    }
}