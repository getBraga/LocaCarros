
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
                          ICarroRepository carroRepository,
                          IAluguelRepository aluguelRepository,
                            IMarcaRepository marcaRepository,
                          IModeloRepository modelos)
        {
            _context = context;
            Vendas = vendaRepository;
            Carros = carroRepository;
            Marcas = marcaRepository;
            Alugueis = aluguelRepository;
            Modelos = modelos;
        }

        public IVendaRepository Vendas { get; private set; }
        public ICarroRepository Carros { get; private set; }
        public IAluguelRepository Alugueis { get; private set; }
        public IMarcaRepository Marcas { get; private set; }
        public IModeloRepository Modelos { get; private set; }
        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {

            if (_transaction == null)
            {
                throw new InvalidOperationException("Transaction has not been started.");
            }

            try
            {
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
            }
            catch
            {
                await _transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await _transaction.DisposeAsync();
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("Transaction has not been started.");
            }

            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
