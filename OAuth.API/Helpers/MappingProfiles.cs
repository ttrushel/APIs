using AutoMapper;
using OAuthAPI.Models;
using OAuthAPI.Requests;

namespace OAuthAPI.Mapper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<RegistrationRequest, Administrator>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.AdminUsername))
                .ForMember(dest => dest.AdminSecret, opt => opt.MapFrom(src => src.AdminPassword));
            CreateMap<RegistrationRequest, Client>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ClientId))
               .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.ClientName))
               .ForMember(dest => dest.ClientSecret, opt => opt.MapFrom(src => src.ClientSecret));
            CreateMap<TokenRequest, ClientInformation>()
              .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.ClientName))
              .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.ClientId))
              .ForMember(dest => dest.ClientSecret, opt => opt.MapFrom(src => src.ClientSecret));
        }
    }
}