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

        public async Task<Modelo> CreateAsync(Modelo modelo)
        {
            _contextModelo.Add(modelo);
            await _contextModelo.SaveChangesAsync();
            return modelo;
        }

        public async Task<bool> DeleteAsync(Modelo modelo)
        {
            _contextModelo.Remove(modelo);
            return await _contextModelo.SaveChangesAsync() > 0;
        }

        public async Task<Modelo?> GetModeloByIdAsync(int id)
        {
           return await _contextModelo.Modelos.FindAsync(id);
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

        public async Task<Modelo> UpdateAsync(Modelo modelo)
        {
            var existingModelo = await _contextModelo.Modelos.FindAsync(modelo.Id);
           if (existingModelo == null)
            {
                throw new KeyNotFoundException("Modelo não encontrado.");
            }
            _contextModelo.Modelos.Update(modelo);
            await _contextModelo.SaveChangesAsync();
            return modelo;
        }
    }
}
