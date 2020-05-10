using System;
using System.Linq;
using AutoMapper;
using DatingApp.API.DTO;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfileExtension : Profile
    {
        public AutoMapperProfileExtension()
        {
            CreateMap<User, UserForListDTO>()
            .ForMember(dest => dest.PhotoURL, opt => 
                opt.MapFrom(src => (src.Photos.FirstOrDefault(p => p.IsMain).Url)))
            .ForMember(dest => dest.Age, opt => 
            opt.MapFrom(src => src.DateOfBirth.CalcularIdade()));
            


            CreateMap<User, UserForDetailedList>()
            .ForMember(dest => dest.PhotoURL, opt => 
                opt.MapFrom(src => (src.Photos.FirstOrDefault(p => p.IsMain).Url)));
            
            
            CreateMap<Photo, PhotoForDetailedDTO>();

            CreateMap<UserForUpdateDTO, User>();             
        }
        
    }
}