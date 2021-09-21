using AutoMapper;
using Project.Domain;
using Project.Domain.Identity;
using Project.WebAPI.Dtos;

namespace Project.WebAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, UserLoginDto>().ReverseMap();
            CreateMap<User, UserAdminDto>().ReverseMap();
            CreateMap<Employee,EmployeeDto>().ReverseMap();
        }
    }
}