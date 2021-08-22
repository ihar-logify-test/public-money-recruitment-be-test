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
            var rental = _rentalRepository.LoadOrNull(bookingModel.RentalId);
            if (rental == null)
                throw new RentalNotFoundException(bookingModel.RentalId);
            
            if (!IsRentalAvailableForBooking(bookingModel, rental))
                throw new ApplicationException("Not available");
            
            var bookingDataModel = _mapper.Map<BookingDataModel>(bookingModel);
            return _bookingRepository.Add(bookingDataModel); 
        }

        private bool IsRentalAvailableForBooking(BookingBindingModel bookingModel, RentalDataModel rentalDataModel)
        {
            var startDate = bookingModel.Start.AddDays(- rentalDataModel.PreparationTimeInDays);
            var endDate = bookingModel.Start.AddDays(bookingModel.Nights + rentalDataModel.PreparationTimeInDays);

            var exitstingBookings = _bookingRepository.LoadBookingsForPeriod(bookingModel.RentalId, startDate, endDate);
            
            return exitstingBookings.SelectMany(_ => _.GetDatesList(preparationDays: rentalDataModel.PreparationTimeInDays))
                                    .GroupBy(_ => _, (key, elements) => elements.Count())
                                    .All(_ => _ < rentalDataModel.Units);
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
            var rental = _rentalRepository.LoadOrNull(getCalendarModel.RentalId);
            if (rental == null)
                throw new RentalNotFoundException(getCalendarModel.RentalId);

            var result = new CalendarViewModel 
            {
                RentalId = getCalendarModel.RentalId,
                Dates = new List<CalendarDateViewModel>() 
            };
            
            var lookupStartDate = getCalendarModel.Start.AddDays(- rental.PreparationTimeInDays).Date;
            var lookupEndDate = getCalendarModel.Start.AddDays(getCalendarModel.Nights + rental.PreparationTimeInDays).Date;
            var calendarBookings = _bookingRepository.LoadBookingsForPeriod(getCalendarModel.RentalId, lookupStartDate, lookupEndDate);

            var allocator = new BookingUnitAllocator(rental.Units);

            for (var i = 0; i < getCalendarModel.Nights; i++)
            {
                result.Dates.Add(CreateCalendarDateViewModel(getCalendarModel, calendarBookings, i, allocator, rental.PreparationTimeInDays));
            }
            
            return result;
        }

        private CalendarDateViewModel CreateCalendarDateViewModel(
            GetCalendarModel getCalendarModel, 
            IEnumerable<BookingDataModel> bookingsInRequestedRange,
            int nightNumber, 
            BookingUnitAllocator allocator,
            int preparationTime)
        {
            var date = new CalendarDateViewModel
            {
                Date = getCalendarModel.Start.Date.AddDays(nightNumber),
                Bookings = new List<CalendarBookingViewModel>(),
                PreparationTimes = new List<CalendarPreparationTimeViewModel>()
            };
            
            var bookingsWithTargetDate = bookingsInRequestedRange.Where(_ => date.Date.IsInRange(_.Start, _.GetEndDate()))
                                                                    .Select(_ => _.Id)
                                                                    .ToList();
            
            var bookingsWithTargetDateInPreparation = bookingsInRequestedRange.Where(_ => date.Date.IsInRange(_.GetPreparationStartDate(), _.GetPreparationEndDate(preparationTime)))
                                                                                .Select(_ => _.Id)
                                                                                .ToList();

            date.Bookings
                .AddRange(bookingsWithTargetDate.Select(_ => new CalendarBookingViewModel { Id = _, Unit = allocator.Allocate(_, bookingsWithTargetDate) }));

            date.PreparationTimes
                .AddRange(bookingsWithTargetDateInPreparation.Select(_ => new CalendarPreparationTimeViewModel { Unit = allocator.Allocate(_, bookingsWithTargetDateInPreparation) }));
            
            return date;
        }

        public RentalViewModel GetRental(int rentalId)
        {
            var rentalDataModel = _rentalRepository.LoadOrNull(rentalId);
            if (rentalDataModel == null) throw new RentalNotFoundException(rentalId);

            return _mapper.Map<RentalViewModel>(rentalDataModel);
        }

        private class BookingUnitAllocator
        {
            private readonly IDictionary<int, int> _bookingIdToUnitMap = new Dictionary<int, int>();
            private readonly int _rentalUnits;

            public BookingUnitAllocator(int rentalUnits)
            {
                _rentalUnits = rentalUnits;
            }

            public int Allocate(int bookingId, IEnumerable<int> bookingIdsForTargetDate)
            {
                if (_bookingIdToUnitMap.TryGetValue(bookingId, out var result)) return result;

                var bookingsWithUnits = bookingIdsForTargetDate.Where(_ => _bookingIdToUnitMap.ContainsKey(_));
                
                var allocatedUnits = bookingsWithUnits.Select(_ => _bookingIdToUnitMap[_]);
                var allUnits = Enumerable.Range(1, _rentalUnits);
                
                var freeUnits = allUnits.Except(allocatedUnits).OrderBy(_ => _);
                result = freeUnits.First();
                _bookingIdToUnitMap[bookingId] = result;
               
               return result;
            }
        }
    }
}