using Microsoft.Extensions.DependencyInjection;
using VacationRental.DAL.InMemory.Repositories;
using VacationRental.DAL.Repositories;

namespace VacationRental.DAL.InMemory.Di
{
    public static class DiExtensions
    {
        public static IServiceCollection AddInMemoryRepositories(this IServiceCollection services)
        {
            services.AddSingleton<IRentalRepository, RentalRepository>();
            services.AddSingleton<IBookingRepository, BookingRepository>();
            
            return services;
        }
    }
}