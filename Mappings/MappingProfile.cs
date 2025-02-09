using AutoMapper;
using ScaffoldDeneme.Models;
using ScaffoldDeneme.DTOs;

namespace ScaffoldDeneme.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Nationality, NationalityDTO>();
            CreateMap<Student, StudentDTO>();
            CreateMap<NationalityDTO, Nationality>();
            CreateMap<StudentDTO, Student>();
        }
    }
}
