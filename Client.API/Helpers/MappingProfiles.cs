using AutoMapper;
using ClientApi.Models;
using ClientApi.Requests;

namespace ClientApi.Mapper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Client, TokenRequest>()
              .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.Id))
              .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.ClientName))
              .ForMember(dest => dest.ClientSecret, opt => opt.MapFrom(src => src.ClientSecret));
            CreateMap<IEnumerable<ClientResponseData>, ClientDataRequest>()
             .ForMember(dest => dest.RequestData, opt => opt.MapFrom(src => src));
        }
    }
}