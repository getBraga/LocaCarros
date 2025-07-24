using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IVendaRepository Vendas { get; }
        ICarroRepository Carros { get; }
        IAluguelRepository Alugueis { get; }
        IMarcaRepository Marcas { get; }
        IModeloRepository Modelos { get; }
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
