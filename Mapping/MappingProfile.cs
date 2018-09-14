using System.Linq;
using AutoMapper;
using FinderApp.API.Dtos;
using FinderApp.API.Helpers;
using FinderApp.API.Model;

namespace FinderApp.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>()
               .ForMember(dest => dest.PhotoUrl, opt => {
                   opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).url);
               })
            .ForMember(dest => dest.Age, opt => {
                opt.ResolveUsing(src => src.DateOfBirth.CalculateAge());
            });

            CreateMap<User, UserDetailedDto>()
            .ForMember(dest => dest.PhotoUrl, opt => {
                   opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).url);
               })
            .ForMember(dest => dest.Age, opt => {
                opt.ResolveUsing(src => src.DateOfBirth.CalculateAge());
            });
            CreateMap<Photo, PhotosDto>();
        }

    }
}