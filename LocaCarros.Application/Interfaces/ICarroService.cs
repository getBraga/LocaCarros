using LocaCarros.Application.DTOs.CarrosDtos;
using LocaCarros.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.Application.Interfaces
{
    public interface ICarroService
    {
        Task<IEnumerable<CarroDTO>> GetCarrosAsync();
        Task<CarroDTO?> GetCarroByIdAsync(int id);
        Task<IEnumerable<CarroDTO>> GetCarrosByModeloIdAsync(int modeloId);
        Task<CarroDTO> CreateAsync(CarroDTOAdd carro);
        Task<CarroDTO> UpdateAsync(CarroDTOUpdate carro);
        Task AtualizarStatusDosCarrosAsync(Carro? carroAnterior, Carro novoCarro);
        Task<Carro> GetEntidadeCarroByIdAsync(int id);
        Task MarcarComoVendidoAsync(Carro carro);
        Task ValidarDisponibilidadeDoCarroAsync(Carro carro);
        Task<bool> DeleteAsync(int id);
    }
}
