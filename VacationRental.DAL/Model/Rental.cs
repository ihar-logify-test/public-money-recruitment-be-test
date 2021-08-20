namespace VacationRental.DAL.Model
{
    public class Rental : IIdentifier<int>
    {
        public int Id { get; set; }
        public int Units { get; set; }
    }
}