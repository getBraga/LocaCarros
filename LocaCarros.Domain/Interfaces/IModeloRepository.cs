using LocaCarros.Domain.Entities;

namespace LocaCarros.Domain.Interfaces
{
    public interface IModeloRepository
    {
        Task<IEnumerable<Modelo>> GetModelosAsync();
        Task<Modelo?> GetModeloByIdAsync(int id);
        Task<Modelo> CreateAsync(Modelo modelo);
        Task<Modelo> UpdateAsync(Modelo modelo);
        Task<bool> DeleteAsync(Modelo modelo);
        Task<IEnumerable<Modelo>> GetModelosByMarcaIdAsync(int marcaId);
        Task<Modelo?> GetModeloByNomeAsync(string nome);
    }
}
