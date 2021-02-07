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
            VehiclesMappings();
        }

        public void UsersMappings() 
        {
            CreateMap<Customer, CustomerDto>();
            CreateMap<Employee, EmployeeDto>();
            CreateMap<User, UserDto>();
            CreateMap<CustomerDto, Customer>();
            CreateMap<EmployeeDto, Employee>();
            CreateMap<UserDto, User>();
        }

        public void VehiclesMappings() 
        {
            CreateMap<Vehicle, VehicleDto>();
            CreateMap<VehicleDto, Vehicle>();
            CreateMap<Make, MakeDto>();
            CreateMap<MakeDto, Make>();
            CreateMap<Model, ModelDto>();
            CreateMap<ModelDto, Model>();
        }
    }
}
