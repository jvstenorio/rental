using AutoMapper;
using Rental.Domain.Entities;
using Rental.Domain.Models;

namespace Rental.Domain.Mappers
{
    public class MapperProfile : Profile
    {
        public MapperProfile() 
        {
            UsersMappings();
        }

        public void UsersMappings() 
        {
            CreateMap<Customer, CustomerDto>();
            CreateMap<Employee, EmployeeDto>();
            CreateMap<User, UserDto>().ForMember(dest => dest.Profile, opt => opt.MapFrom(src => src.Profile.ToString()));
            CreateMap<CustomerDto, Customer>();
            CreateMap<EmployeeDto, Employee>();
            CreateMap<UserDto, User>();
        }
    }
}
