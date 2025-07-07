using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.Domain.Entities
{
    public sealed class Venda
    {
        public int Id { get; private set; }
        public decimal ValorVenda { get; private set; }
        public DateTime DataVenda { get; private set; }
        public int CarroId { get; private set; }
        public Carro Carro { get; private set; } = null!;
        public byte[] RowVersion { get; set; } = [];

        [ExcludeFromCodeCoverage]
        private Venda() { }

        public Venda(decimal valorVenda, DateTime dataVenda, Carro carro)
        {

            SetValorVenda(valorVenda);
            SetDataVenda(dataVenda);
            SetCarro(carro);
          
        }

        public void SetRowVersion(byte[] rowVersion) { 
            RowVersion = rowVersion;
        }

        public void Update(decimal valorVenda, DateTime dataVenda, Carro carro)
        {
            if (Id < 0)
                throw new InvalidOperationException("A Venda deve conter um Id válido.");
            SetValorVenda(valorVenda);
            SetDataVenda(dataVenda);
            SetCarro(carro);
        }
      
        public void SetValorVenda(decimal valorVenda)
        {
            if (valorVenda <= 0)
            {
                throw new ArgumentException("O valor da venda deve ser maior que zero.", nameof(valorVenda));
            }
            ValorVenda = valorVenda;
        }
        public void SetDataVenda(DateTime dataVenda)
        {
            if (dataVenda == default)
            {
                throw new ArgumentException("Data de venda inválida.", nameof(dataVenda));
            }
            if (dataVenda > DateTime.Now)
            {
                throw new ArgumentException("A data da venda não pode ser no futuro.", nameof(dataVenda));
            }
            DataVenda = dataVenda;
        }
        public void SetCarro(Carro carro)
        {
            Carro = carro ?? throw new ArgumentNullException(nameof(carro), "O carro não pode ser nulo.");
            CarroId = carro.Id;
        }

    }
}
