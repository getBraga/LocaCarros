using LocaCarros.Domain.Entities;

namespace LocaCarros.Domain.Interfaces
{
    public interface IMarcaRepository
    {
        Task<IEnumerable<Marca>> GetMarcasAsync();
        Task<Marca?> GetMarcaByIdAsync(int id);
        Task<Marca> CreateAsync(Marca marca);
        Task<Marca> UpdateAsync(Marca marca);
        Task<bool> DeleteAsync(Marca marca);
    }
}
