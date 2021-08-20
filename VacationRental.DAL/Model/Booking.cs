using System;

namespace VacationRental.DAL.Model
{
    public class Booking : IIdentifier<int>
    {
        public int Id { get; set; }
        public int RentalId { get; set; }
        public DateTime Start { get; set; }
        public int Nights { get; set; }
    }
}