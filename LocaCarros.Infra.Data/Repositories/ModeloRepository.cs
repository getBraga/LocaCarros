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
    public class ModeloRepository(ApplicationDbContext contextModelo) : IModeloRepository
    {
        private readonly ApplicationDbContext _contextModelo = contextModelo;

        public Task<Modelo> CreateAsync(Modelo modelo)
        {
            _contextModelo.Add(modelo);
            return Task.FromResult(modelo);
        }

        public  Task<bool> DeleteAsync(Modelo modelo)
        {
            _contextModelo.Remove(modelo);
            return Task.FromResult(true);
        }

        public async Task<Modelo?> GetModeloByIdAsync(int id)
        {
           return await _contextModelo.Modelos.FindAsync(id);
        }

        public async Task<Modelo?> GetModeloByNomeAsync(string nome)
        {
            var modelo = await _contextModelo.Modelos.FirstOrDefaultAsync(m => m.Nome.Equals(nome, StringComparison.CurrentCultureIgnoreCase));
            return modelo;
        }

        public async Task<IEnumerable<Modelo>> GetModelosAsync()
        {
           return await _contextModelo.Modelos.Include(m => m.Marca)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Modelo>> GetModelosByMarcaIdAsync(int marcaId)
        {
            return await _contextModelo.Modelos.Include(m => m.Marca)
                .AsNoTracking()
                .Where(m => m.MarcaId == marcaId).ToListAsync(); ;
        }

        public Task<Modelo> UpdateAsync(Modelo modelo)
        {
            _contextModelo.Modelos.Update(modelo);
            return Task.FromResult(modelo);
        }
    }
}
