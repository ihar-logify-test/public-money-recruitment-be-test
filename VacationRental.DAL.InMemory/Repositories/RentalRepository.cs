using VacationRental.DAL.Model;
using VacationRental.DAL.Repositories;

namespace VacationRental.DAL.InMemory.Repositories
{
    public class RentalRepository : InMemoryRepository<RentalDataModel>, IRentalRepository
    {
    }
}