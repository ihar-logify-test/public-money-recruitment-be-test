namespace VacationRental.BLL.Exceptions
{
    public class RentalNotFoundException : NotFoundException<int>
    {
        public RentalNotFoundException(int rentalId) : base(rentalId, "Rental")
        {
        }
    }
}