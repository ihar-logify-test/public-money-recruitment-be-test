using System.Linq;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using VacationRental.DAL.Repositories;
using VacationRental.DAL.Model;
using VacationRental.Contract.Models;
using VacationRental.BLL.Services.Interfaces;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/bookings")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IRentalService _rentalService;

        public BookingsController(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        [HttpGet]
        [Route("{bookingId:int}")]
        public BookingViewModel Get(int bookingId)
        {
            return _rentalService.GetBooking(bookingId);
        }

        [HttpPost]
        public ResourceIdViewModel Post(BookingBindingModel model)
        {
            var id = _rentalService.CreateBooking(model);
            return new ResourceIdViewModel { Id = id };
        }
    }
}
