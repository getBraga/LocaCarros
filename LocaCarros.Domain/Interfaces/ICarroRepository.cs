using LocaCarros.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.Domain.Interfaces
{
    public interface ICarroRepository
    {
        Task<IEnumerable<Carro>> GetCarrosAsync();
        Task<Carro> GetCarroByIdAsync(int id);
        Task<Carro> CreateAsync(Carro carro);
        Task<Carro> UpdateAsync(Carro carro);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Carro>> GetCarrosByModeloIdAsync(int modeloId);
    
    }
}
