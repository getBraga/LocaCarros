using LocaCarros.Application.DTOs.ModelosDtos;
using LocaCarros.Domain.Entities;

namespace LocaCarros.Application.Interfaces
{
    public interface IModeloService
    {
        Task<IEnumerable<ModeloDTO>> GetModelosAsync();
        Task<ModeloDTO?> GetByIdAsync(int id);
        Task<IEnumerable<ModeloDTO>> GetByMarcaIdAsync(int marcaId);
        Task<ModeloDTO> AddAsync(ModeloDTOAdd modeloDto);
        Task<ModeloDTO> UpdateAsync(ModeloDTOUpdate modeloDto);
        Task<bool> RemoveAsync(int id);
    }
}
