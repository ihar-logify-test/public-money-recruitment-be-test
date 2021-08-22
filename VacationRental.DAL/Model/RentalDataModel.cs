namespace VacationRental.DAL.Model
{
    public class RentalDataModel : IIdentifier<int>
    {
        public int Id { get; set; }
        public int Units { get; set; }
        public int PreparationTimeInDays { get; set; }
    }
}