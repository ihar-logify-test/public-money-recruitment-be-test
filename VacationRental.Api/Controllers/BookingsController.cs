using System.Linq;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;
using VacationRental.DAL.Repositories;
using VacationRental.DAL.Model;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/bookings")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRentalRepository _rentalRepository;
        
        public BookingsController(
            IBookingRepository bookingRepository, 
            IRentalRepository rentalRepository)
        {
            _bookingRepository = bookingRepository;
            _rentalRepository = rentalRepository;
        }

        [HttpGet]
        [Route("{bookingId:int}")]
        public BookingViewModel Get(int bookingId)
        {
            var booking = _bookingRepository.Load(bookingId);
            return new BookingViewModel {Id = booking.Id, Nights = booking.Nights, Start = booking.Start, RentalId = booking.RentalId };
        }

        [HttpPost]
        public ResourceIdViewModel Post(BookingBindingModel model)
        {
            if (model.Nights <= 0)
                throw new ApplicationException("Nigts must be positive");
            if (!_rentalRepository.LoadAll().Any(_ => _.Id == model.RentalId))
                throw new ApplicationException("Rental not found");

            for (var i = 0; i < model.Nights; i++)
            {
                var count = 0;
                foreach (var booking in _bookingRepository.LoadAll())
                {
                    if (booking.RentalId == model.RentalId
                        && (booking.Start <= model.Start.Date && booking.Start.AddDays(booking.Nights) > model.Start.Date)
                        || (booking.Start < model.Start.AddDays(model.Nights) && booking.Start.AddDays(booking.Nights) >= model.Start.AddDays(model.Nights))
                        || (booking.Start > model.Start && booking.Start.AddDays(booking.Nights) < model.Start.AddDays(model.Nights)))
                    {
                        count++;
                    }
                }
                if (count >= _rentalRepository.Load(model.RentalId).Units)
                    throw new ApplicationException("Not available");
            }
            
            var bookingToAdd = new Booking {Nights = model.Nights, Start = model.Start, RentalId = model.RentalId };
            var id = _bookingRepository.Add(bookingToAdd);
            return new ResourceIdViewModel { Id = id };
        }
    }
}
