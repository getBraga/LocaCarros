using LocaCarros.Domain.Entities;

namespace LocaCarros.Domain.Interfaces
{
    public interface IVendaRepository
    {
        Task<IEnumerable<Venda>> GetVendasAsync();
        Task<Venda?> GetVendaByIdAsync(int id);
        Task<Venda> CreateAsync(Venda venda);
        Task<Venda> UpdateAsync(Venda venda);
        Task<bool> DeleteAsync(Venda venda);
        Task<IEnumerable<Venda>> GetVendasByCarroIdAsync(int carroId);
    }
}
