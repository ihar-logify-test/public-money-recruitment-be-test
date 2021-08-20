using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;
using VacationRental.DAL.Model;
using VacationRental.DAL.Repositories;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/rentals")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly IRentalRepository _rentalRepository;

        public RentalsController(IRentalRepository rentalRepository)
        {
            _rentalRepository = rentalRepository;
        }

        [HttpGet]
        [Route("{rentalId:int}")]
        public RentalViewModel Get(int rentalId)
        {
            var rental = _rentalRepository.Load(rentalId);
            return new RentalViewModel {Id = rental.Id, Units = rental.Units} ;
        }

        [HttpPost]
        public ResourceIdViewModel Post(RentalBindingModel model)
        {
            var rental = new Rental { Units = model.Units };
            _rentalRepository.Add(rental);

            return new ResourceIdViewModel { Id = rental.Id };
        }
    }
}
