using AutoMapper;

using VacationRental.Contract.Models;
using VacationRental.DAL.Model;

namespace VacationRental.BLL.Mapping.Profiles
{
    public class DataModelProfile : Profile
    {
        public DataModelProfile()
        {
            CreateMap<BookingBindingModel, BookingDataModel>();
            CreateMap<BookingDataModel, BookingViewModel>();
            
            CreateMap<RentalBindingModel, RentalDataModel>();
            CreateMap<RentalDataModel, RentalViewModel>();
        }
    }
}