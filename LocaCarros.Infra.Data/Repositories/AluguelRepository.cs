using LocaCarros.Domain.Entities;
using LocaCarros.Domain.Interfaces;
using LocaCarros.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.Infra.Data.Repositories
{
    public class AluguelRepository : IAluguelRepository
    {
        private readonly ApplicationDbContext _contextAluguel;
        public AluguelRepository(ApplicationDbContext contextAluguel)
        {
            _contextAluguel = contextAluguel;
        }
        public  Task<Aluguel> CreateAsync(Aluguel aluguel)
        {
            _contextAluguel.Add(aluguel);
            return Task.FromResult(aluguel);
        }

        public async Task<bool> DeleteAsync(Aluguel aluguel)
        {
            _contextAluguel.Remove(aluguel);
            return await _contextAluguel.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Aluguel>> GetAlugueisAsync()
        {
            return await _contextAluguel.Alugueis
                   .Include(x => x.Carro)
                   .ThenInclude(x => x.Modelo)
                   .AsNoTracking()
                   .ToListAsync();

        }

        public async Task<IEnumerable<Aluguel>> GetAlugueisByCarroIdAsync(int carroId)
        {
           return await _contextAluguel.Alugueis.Include(a => a.Carro)
                  .ThenInclude(c => c.Modelo)
                  .ThenInclude(m => m.Marca).Where(a => a.CarroId == carroId).ToListAsync();


        }

        public async Task<Aluguel?> GetAluguelByIdAsync(int id)
        {
            return await _contextAluguel.Alugueis
                .Include(x => x.Carro)
                .ThenInclude(x => x.Modelo)
                .FirstOrDefaultAsync();
        }

        public Task<Aluguel> UpdateAsync(Aluguel aluguel)
        {
            _contextAluguel.Alugueis.Update(aluguel);
            return Task.FromResult(aluguel);
        }
    }
}
