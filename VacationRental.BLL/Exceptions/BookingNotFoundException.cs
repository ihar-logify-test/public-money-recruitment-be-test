namespace VacationRental.BLL.Exceptions
{
    public class BookingNotFoundException : NotFoundException<int>
    {
        public BookingNotFoundException(int resourceId) : base(resourceId, "Booking")
        {
        }
    }
}