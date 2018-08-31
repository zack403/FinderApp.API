using AutoMapper;
using FinderApp.API.Dtos;
using FinderApp.API.Model;

namespace FinderApp.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserForRegisterDto>();
        }

    }
}