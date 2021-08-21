using AutoMapper;
using VacationRental.Contract.Models;
using VacationRental.DAL.Model;

namespace VacationRental.BLL.Mapping.Profiles
{
    public class DataModelProfile : Profile
    {
        public DataModelProfile()
        {
            CreateMap<BookingBindingModel, Booking>();
            CreateMap<Booking, BookingViewModel>();
            
            CreateMap<RentalBindingModel, Rental>();
            CreateMap<Rental, RentalViewModel>();
        }
    }
}