using Microsoft.AspNetCore.Mvc;

using VacationRental.BLL.Services.Interfaces;
using VacationRental.Contract.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/rentals")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly IRentalService _rentalService;

        public RentalsController(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        [HttpGet]
        [Route("{rentalId:int}")]
        public RentalViewModel Get(int rentalId)
        {
            return _rentalService.GetRental(rentalId);
        }

        [HttpPost]
        public ResourceIdViewModel Post(RentalBindingModel model)
        {
            var id = _rentalService.CreateRental(model);
            return new ResourceIdViewModel { Id = id };
        }
        
        [HttpPut]
        [Route("{id:int}")]
        public void Put(int id, RentalBindingModel model)
        {
            _rentalService.UpdateRental(id, model);
        }
    }
}
