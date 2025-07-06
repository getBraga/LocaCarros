using AutoMapper;
using LocaCarros.Application.DTOs.MarcasDtos;
using LocaCarros.Application.DTOs.ModelosDtos;
using LocaCarros.Domain.Entities;

namespace LocaCarros.Application.Mappings
{
    public class DomainToDTOMappingProfile : Profile
    {
        public DomainToDTOMappingProfile()
        {
            CreateMap<Marca, MarcaDTO>().ReverseMap();
            CreateMap<Marca, MarcaDTOAdd>().ReverseMap();
            CreateMap<Modelo, ModeloDTO>().ReverseMap();
            CreateMap<Modelo, ModeloDTOAdd>().ReverseMap();
            CreateMap<Modelo, ModeloDTOUpdate>().ReverseMap();



        }
    }
}
