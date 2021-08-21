using System;

using Microsoft.AspNetCore.Mvc;

using VacationRental.BLL.Services.Interfaces;
using VacationRental.Contract.Models;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/calendar")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly IRentalService _rentalService;

        public CalendarController(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        [HttpGet]
        public CalendarViewModel Get([FromQuery] GetCalendarModel getCalendarModel)
        {
            return _rentalService.GetCalendar(getCalendarModel);
        }
    }
}
