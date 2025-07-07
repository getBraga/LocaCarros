using LocaCarros.Application.Interfaces;
using LocaCarros.Domain.Interfaces;
using LocaCarros.Infra.Data.Context;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LocaCarros.Infra.Data.Transaction
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(ApplicationDbContext context,
                          IVendaRepository vendaRepository,
                          ICarroRepository carroRepository)
        {
            _context = context;
            Vendas = vendaRepository;
            Carros = carroRepository;
        }

        public IVendaRepository Vendas { get; private set; }
        public ICarroRepository Carros { get; private set; }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _transaction.CommitAsync();
        }

        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
