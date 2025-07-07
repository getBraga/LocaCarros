using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IVendaRepository Vendas { get; }
        ICarroRepository Carros { get; }
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
