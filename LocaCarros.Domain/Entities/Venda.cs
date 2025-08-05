using LocaCarros.Domain.Exceptions;


using System.Diagnostics.CodeAnalysis;
using System.Globalization;


namespace LocaCarros.Domain.Entities
{
    public sealed class Venda
    {
        public int Id { get; private set; }
        public decimal ValorVenda { get; private set; }
        public DateTime DataVenda { get; private set; }
        public int CarroId { get; private set; }
        public Carro Carro { get; private set; } = null!;
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

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
            if (Id <= 0)
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
                throw new DomainException("Data de venda inválida.");
            }
            if (dataVenda > DateTime.Now)
            {
                throw new DomainException("A data da venda não pode ser no futuro.");
            }
            DataVenda = dataVenda;
        }

    


        public void SetCarro(Carro? carro)
        {
            Carro = carro ?? throw new DomainException("O carro não pode ser nulo.");
            CarroId = carro.Id;
        }

    }
}
