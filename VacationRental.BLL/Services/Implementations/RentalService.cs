using System.Linq;
using System;
using System.Collections.Generic;

using AutoMapper;

using VacationRental.BLL.Extensions;
using VacationRental.BLL.Services.Interfaces;
using VacationRental.Contract.Models;
using VacationRental.DAL.Extensions;
using VacationRental.DAL.Model;
using VacationRental.DAL.Repositories;
using VacationRental.BLL.Exceptions;

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
            if (!_rentalRepository.Exists(bookingModel.RentalId))
                throw new RentalNotFoundException(bookingModel.RentalId);

            var bookedUnitsCount = _bookingRepository.BookedUnitsCount(bookingModel.RentalId, bookingModel.Start, bookingModel.Start.AddDays(bookingModel.Nights));
            if (bookedUnitsCount >= _rentalRepository.LoadOrNull(bookingModel.RentalId).Units)
                throw new ApplicationException("Not available");
            
            var bookingDataModel = _mapper.Map<BookingDataModel>(bookingModel);
            return _bookingRepository.Add(bookingDataModel); 
        }
        
        public int CreateRental(RentalBindingModel rentalModel)
        {
            var rentalDataModel = _mapper.Map<RentalDataModel>(rentalModel);
            return _rentalRepository.Add(rentalDataModel);
        }

        public BookingViewModel GetBooking(int bookingId)
        {
            var bookingDataModel = _bookingRepository.LoadOrNull(bookingId);
            if (bookingDataModel == null) throw new BookingNotFoundException(bookingId);
            
            return _mapper.Map<BookingViewModel>(bookingDataModel);
        }
        
        public CalendarViewModel GetCalendar(GetCalendarModel getCalendarModel)
        {
            if (!_rentalRepository.Exists(getCalendarModel.RentalId))
                throw new RentalNotFoundException(getCalendarModel.RentalId);

            var result = new CalendarViewModel 
            {
                RentalId = getCalendarModel.RentalId,
                Dates = new List<CalendarDateViewModel>() 
            };
            
            var start = getCalendarModel.Start.Date;
            for (var i = 0; i < getCalendarModel.Nights; i++)
            {
                var date = new CalendarDateViewModel
                {
                    Date = start.Date.AddDays(i),
                    Bookings = new List<CalendarBookingViewModel>()
                };

                var calendarBookings = _bookingRepository.LoadBookingsForPeriod(getCalendarModel.RentalId, start, start.AddDays(getCalendarModel.Nights))
                                                            .Where(_ => date.Date.IsInRange(_.Start, _.GetEndDate()))
                                                            .Select(_ => new CalendarBookingViewModel { Id = _.Id });

                date.Bookings.AddRange(calendarBookings);
                result.Dates.Add(date);
            }

            return result;
        }

        public RentalViewModel GetRental(int rentalId)
        {
            var rentalDataModel = _rentalRepository.LoadOrNull(rentalId);
            if (rentalDataModel == null) throw new RentalNotFoundException(rentalId);

            return _mapper.Map<RentalViewModel>(rentalDataModel);
        }
    }
}