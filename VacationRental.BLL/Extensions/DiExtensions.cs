using Microsoft.Extensions.DependencyInjection;

using VacationRental.BLL.Services.Implementations;
using VacationRental.BLL.Services.Interfaces;

namespace VacationRental.BLL.Extensions
{
    public static class DiExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddTransient<IRentalService, RentalService>();
            
            return services;
        }
    }
}