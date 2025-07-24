using LocaCarros.Domain.Entities;
using LocaCarros.Domain.Interfaces;
using LocaCarros.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;


namespace LocaCarros.Infra.Data.Repositories
{
    public class MarcaRepository : IMarcaRepository
    {
        private readonly ApplicationDbContext _contextMarca;
        public MarcaRepository(ApplicationDbContext contextMarca)
        {
            _contextMarca = contextMarca;
        }
        public  Task<Marca> CreateAsync(Marca marca)
        {
            _contextMarca.Add(marca);
            return Task.FromResult(marca);

        }

        public  Task<bool> DeleteAsync(Marca marca)
        {
            _contextMarca.Remove(marca);
            return Task.FromResult(true);
        }

        public async Task<Marca?> GetMarcaByIdAsync(int id)
        {
            return await _contextMarca.Marcas.FindAsync(id);
        }

        public async Task<IEnumerable<Marca>> GetMarcasAsync()
        {
           return await _contextMarca.Marcas
                .AsNoTracking()
                .ToListAsync();
        }

        public  Task<Marca> UpdateAsync(Marca marca)
        {
            _contextMarca.Marcas.Update(marca);
            return Task.FromResult(marca);
        }
    }
}
