using AutoMapper;
using Project.Domain;
using Project.WebAPI.Dtos;

namespace Project.WebAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Employee,EmployeeDto>().ReverseMap();
        }
    }
}