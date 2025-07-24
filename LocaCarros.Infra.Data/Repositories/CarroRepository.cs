using LocaCarros.Domain.Entities;
using LocaCarros.Domain.Interfaces;
using LocaCarros.Domain.Enuns;
using LocaCarros.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.Infra.Data.Repositories
{
    public class CarroRepository : ICarroRepository
    {
        private readonly ApplicationDbContext _contextCarro;
        public CarroRepository(ApplicationDbContext contextCarro)
        {
            _contextCarro = contextCarro;
        }
        public  Task<Carro> CreateAsync(Carro carro)
        {
           _contextCarro.Add(carro);
           return Task.FromResult(carro);
        }

        public  Task<bool> DeleteAsync(Carro carro)
        {
           _contextCarro.Remove(carro);
             return Task.FromResult(true); ;
        }

        public async Task<Carro?> GetCarroByIdAsync(int id)
        {
         return await _contextCarro.Carros.FindAsync(id);
        }

        public async Task<IEnumerable<Carro>> GetCarrosAsync()
        {
         return await _contextCarro.Carros.Include(x => x.Modelo).ThenInclude(x => x.Marca)
                .AsNoTracking()
                .ToListAsync();

        }

        public async Task<IEnumerable<Carro>> GetCarrosByModeloIdAsync(int modeloId)
        {
         return await _contextCarro.Carros.Include(c => c.Modelo)
                .ThenInclude(m => m.Marca)
                .AsNoTracking()
                .Where(c => c.ModeloId == modeloId).ToListAsync();
         
        }

        public Task<Carro> UpdateAsync(Carro carro)
        {
            _contextCarro.Carros.Update(carro);
            return Task.FromResult(carro);
        }

        public Task<IEnumerable<Carro>> UpdatesListAsync(IEnumerable<Carro> carros)
        {
            _contextCarro.Carros.UpdateRange(carros);
            return Task.FromResult(carros);
        }
    }
}
