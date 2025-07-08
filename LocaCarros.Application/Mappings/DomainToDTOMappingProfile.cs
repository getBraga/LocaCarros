using AutoMapper;
using LocaCarros.Application.DTOs.AlugueisDtos;
using LocaCarros.Application.DTOs.CarrosDtos;
using LocaCarros.Application.DTOs.MarcasDtos;
using LocaCarros.Application.DTOs.ModelosDtos;
using LocaCarros.Application.DTOs.VendasDtos;
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
            CreateMap<Carro, CarroDTOAdd>().ReverseMap();
            CreateMap<Carro, CarroDTOUpdate>().ReverseMap();
            CreateMap<Venda, VendaDTO>()
                .ForMember(dest => dest.ModeloNome, opt => opt.MapFrom(src => src.Carro.Modelo.Nome)).ReverseMap();
            CreateMap<Venda, VendaDTOAdd>().ReverseMap();
            CreateMap<Venda, VendaDTOUpdate>().ReverseMap();
            CreateMap<Carro, CarroDTO>()
            .ForMember(dest => dest.MarcaNome, opt => opt.MapFrom(src => src.Modelo.Marca.Nome)).ReverseMap();

            CreateMap<Aluguel, AluguelDTO>().ForMember(dest => dest.ModeloNome, opt => opt.MapFrom(src => src.Carro.Modelo.Nome)).ReverseMap();
            CreateMap<Aluguel, AluguelDTOAdd>().ReverseMap();
            CreateMap<Aluguel, AluguelDTOUpdate>().ReverseMap();
        }
    }
}
