using LocaCarros.Application.DTOs.ModelosDtos;

namespace LocaCarros.Application.Interfaces
{
    public interface IModeloService
    {
        Task<IEnumerable<ModeloDTO>> GetModelosAsync();
        Task<ModeloDTO?> GetByIdAsync(int id);
        Task<ModeloDTO> AddAsync(ModeloDTOAdd modeloDto);
        Task<ModeloDTO> UpdateAsync(ModeloDTOUpdate modeloDto);
        Task<bool> RemoveAsync(int id);
    }
}
