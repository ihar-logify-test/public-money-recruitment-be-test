using System.Linq;
using System.Collections.Generic;

using AutoMapper;

using VacationRental.BLL.Extensions;
using VacationRental.BLL.Services.Interfaces;
using VacationRental.Contract.Models;
using VacationRental.DAL.Extensions;
using VacationRental.DAL.Model;
using VacationRental.DAL.Repositories;
using VacationRental.BLL.Exceptions;
using Microsoft.Extensions.Logging;

namespace VacationRental.BLL.Services.Implementations
{
    public class RentalService : IRentalService
    {
        private readonly IRentalRepository _rentalRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;


        public RentalService(
            IBookingRepository bookingRepository,
            IRentalRepository rentalRepository,
            IMapper mapper, 
            ILogger<RentalService> logger)
        {
            _bookingRepository = bookingRepository;
            _rentalRepository = rentalRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public int CreateBooking(BookingBindingModel bookingModel)
        {
            var rental = _rentalRepository.LoadOrNull(bookingModel.RentalId);
            if (rental == null)
                throw new RentalNotFoundException(bookingModel.RentalId);
            
            if (!IsRentalAvailableForBooking(bookingModel, rental))
                throw new OperationNotAvailableException("The rental is fully booked for the requested dates.");
            
            var bookingDataModel = _mapper.Map<BookingDataModel>(bookingModel);
            
            var id = _bookingRepository.Add(bookingDataModel); 
            _logger.LogInformation("Booking with id '{id}' created.", id);
            return id;
        }

        private bool IsRentalAvailableForBooking(BookingBindingModel bookingModel, RentalDataModel rentalDataModel)
        {
            var startDate = bookingModel.Start.AddDays(- rentalDataModel.PreparationTimeInDays);
            var endDate = bookingModel.Start.AddDays(bookingModel.Nights + rentalDataModel.PreparationTimeInDays);

            var exitstingBookings = _bookingRepository.LoadBookingsForPeriod(bookingModel.RentalId, startDate, endDate);
            return !IsFullyBooked(exitstingBookings, rentalDataModel.Units, rentalDataModel.PreparationTimeInDays);
        }

        private bool IsFullyBooked(IEnumerable<BookingDataModel> bookings, int rentalUnitsCount, int preparationDays)
        {
            return DoOverlappringExceedUnitCount(bookings, rentalUnitsCount - 1, preparationDays);
        }
        
        private bool DoOverlappringExceedUnitCount(IEnumerable<BookingDataModel> bookings, int rentalUnitsCount, int preparationDays)
        {
            return bookings.SelectMany(_ => _.GetDatesList(preparationDays: preparationDays))
                                    .GroupBy(_ => _, (key, elements) => elements.Count())
                                    .Any(_ => _ > rentalUnitsCount);
        }
        
        public int CreateRental(RentalBindingModel rentalModel)
        {
            var rentalDataModel = _mapper.Map<RentalDataModel>(rentalModel);
            
            var id = _rentalRepository.Add(rentalDataModel);
            _logger.LogInformation("Rental with id '{id}' created.", id);
            return id;
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

        public void UpdateRental(int id, RentalBindingModel rental)
        {
            var bookings = _bookingRepository.LoadBookingsForRental(id);
            if (DoOverlappringExceedUnitCount(bookings, rental.Units, rental.PreparationTimeInDays))
            {
                throw new OperationNotAvailableException($"Can't update rental due to either '{nameof(rental.Units)}'"
                                                         + $" or '{nameof(rental.PreparationTimeInDays)}' values conflict with"
                                                        + " existing bookings.");
            }
            
            var rentalDataModel = _mapper.Map<RentalDataModel>(rental);
            rentalDataModel.Id = id;
            
            _rentalRepository.Update(rentalDataModel);
            _logger.LogInformation("Rental with id '{id}' updated.", id);
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