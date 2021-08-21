using System;

namespace VacationRental.Contract.Models
{
    public class GetCalendarModel
    {
        public int RentalId { get; set; }
        public DateTime Start { get; set; }
        public int Nights { get; set; }
    }
}