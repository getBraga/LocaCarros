using LocaCarros.Domain.Entities;

namespace LocaCarros.Domain.Interfaces
{
    public interface IAluguelRepository
    {
        Task<IEnumerable<Aluguel>> GetAlugueisAsync();
        Task<Aluguel?> GetAluguelByIdAsync(int id);
        Task<Aluguel> CreateAsync(Aluguel aluguel);
        Task<Aluguel> UpdateAsync(Aluguel aluguel);
        Task<bool> DeleteAsync(Aluguel aluguel);
        Task<IEnumerable<Aluguel>> GetAlugueisByCarroIdAsync(int carroId);
    }
}
