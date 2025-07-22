using LocaCarros.Domain.Entities;
using LocaCarros.Domain.Interfaces;
using LocaCarros.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;


namespace LocaCarros.Infra.Data.Repositories
{
    public class VendaRepository : IVendaRepository
    {
        private readonly ApplicationDbContext _contextVenda;
        public VendaRepository(ApplicationDbContext contextVenda)
        {
            _contextVenda = contextVenda;
        }

        public async Task<Venda> CreateAsync(Venda venda)
        {
            _contextVenda.Add(venda);
            await _contextVenda.SaveChangesAsync();
           
            return venda;
        }

        public async Task<bool> DeleteAsync(Venda venda)
        {
            _contextVenda.Remove(venda);
            return await _contextVenda.SaveChangesAsync() > 0;
        }

        public async Task<Venda?> GetVendaByIdAsync(int id)
        {
            return await _contextVenda.Vendas
                .Include(x => x.Carro)
                .ThenInclude(x => x.Modelo)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Venda>> GetVendasAsync()
        {
            return await _contextVenda.Vendas.AsNoTracking().Include(x => x.Carro).ThenInclude(x => x.Modelo).ToListAsync();

        }

        public async Task<IEnumerable<Venda>> GetVendasByCarroIdAsync(int carroId)
        {
            return await _contextVenda.Vendas.Include(v => v.Carro)
                .ThenInclude(c => c.Modelo)
                .ThenInclude(m => m.Marca)
                .Where(v => v.CarroId == carroId).ToListAsync();

        }

        public async Task<Venda> UpdateAsync(Venda venda)
        {
            _contextVenda.Vendas.Update(venda);
            await _contextVenda.SaveChangesAsync();
            return venda;
        }
    }
}
