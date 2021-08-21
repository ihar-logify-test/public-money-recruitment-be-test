using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public RentalService(
            IBookingRepository bookingRepository,
            IRentalRepository rentalRepository, 
            IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _rentalRepository = rentalRepository;
            _mapper = mapper;
        }

        public int CreateBooking(BookingBindingModel bookingModel)
        {
            if (bookingModel.Nights <= 0)
                throw new ApplicationException("Nigts must be positive");
            if (!_rentalRepository.LoadAll().Any(_ => _.Id == bookingModel.RentalId))
                throw new ApplicationException("Rental not found");

            for (var i = 0; i < bookingModel.Nights; i++)
            {
                var count = 0;
                foreach (var booking in _bookingRepository.LoadAll())
                {
                    if (booking.RentalId == bookingModel.RentalId
                        && (booking.Start <= bookingModel.Start.Date && booking.Start.AddDays(booking.Nights) > bookingModel.Start.Date)
                        || (booking.Start < bookingModel.Start.AddDays(bookingModel.Nights) && booking.Start.AddDays(booking.Nights) >= bookingModel.Start.AddDays(bookingModel.Nights))
                        || (booking.Start > bookingModel.Start && booking.Start.AddDays(booking.Nights) < bookingModel.Start.AddDays(bookingModel.Nights)))
                    {
                        count++;
                    }
                }
                if (count >= _rentalRepository.Load(bookingModel.RentalId).Units)
                    throw new ApplicationException("Not available");
            }
            
            var bookingToAdd = _mapper.Map<Booking>(bookingModel);
            return _bookingRepository.Add(bookingToAdd); 
        }

        public int CreateRental(RentalBindingModel rentalModel)
        {
            var rental = _mapper.Map<Rental>(rentalModel);
            return _rentalRepository.Add(rental);
        }

        public BookingViewModel GetBooking(int bookingId)
        {
            var booking = _bookingRepository.Load(bookingId);
            return _mapper.Map<BookingViewModel>(booking);
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
            return _mapper.Map<RentalViewModel>(rental);
        }
    }
}