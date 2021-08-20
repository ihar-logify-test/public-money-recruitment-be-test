using VacationRental.DAL.Model;
using VacationRental.DAL.Repositories;

namespace VacationRental.DAL.InMemory.Repositories
{
    public class BookingRepository : InMemoryRepository<Booking>, IBookingRepository
    {
    }
}