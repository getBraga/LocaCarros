using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.Domain.Entities
{
    public sealed class Aluguel
    {
        
        public int Id { get; private set; } 
     
        public decimal ValorAluguel { get; private set; }
        public DateTime DataInicio { get; private set; }
        public DateTime DataDevolucao { get; private set; }

        public int CarroId { get; private set; }
        public Carro Carro { get; private set; } = null!;

        [ExcludeFromCodeCoverage]
        private Aluguel() { }
 
        public Aluguel(decimal valorAluguel, DateTime dataInicio, DateTime dataDevolucao, Carro carro)
        {
            SetValorAluguel(valorAluguel);
            SetDataInicio(dataInicio);
            SetDataDevolucao(dataDevolucao);
            SetCarro(carro);

        }
        public void Update(decimal valorAluguel, DateTime dataInicio, DateTime DataDevolucaoim, Carro carro)
        {
            if (Id < 0)
                throw new InvalidOperationException("O Aluguel deve conter um Id válido.");
            SetValorAluguel(valorAluguel);
            SetDataInicio(dataInicio);
            SetDataDevolucao(DataDevolucaoim);
            SetCarro(carro);
        }
     

        public void SetValorAluguel(decimal valorAluguel)
        {
            if (valorAluguel <= 0)
                throw new ArgumentException("O valor do aluguel deve ser maior que zero.", nameof(valorAluguel));
            ValorAluguel = valorAluguel;
        }
        public void SetDataInicio(DateTime dataInicio)
        {
            if (dataInicio == default)
                throw new ArgumentException("Data de início inválida.", nameof(dataInicio));
            DataInicio = dataInicio;
        }
        public void TrocarCarro(Carro novoCarro, DateTime dataInicio, DateTime dataFim, decimal valor)
        {
            SetCarro(novoCarro);
            SetDataInicio(dataInicio);
            SetDataDevolucao(dataFim);
            SetValorAluguel(valor);
        }
        public void SetDataDevolucao(DateTime dataDevolucao)
        {
            if (dataDevolucao == default)
                throw new ArgumentException("Data de devolucao é inválida.", nameof(dataDevolucao));
            if (dataDevolucao <= DataInicio)
                throw new ArgumentException("A data de devolução não pode ser anterior à data de início do aluguel.", nameof(dataDevolucao));
            DataDevolucao = dataDevolucao;
        }
        public void SetCarro(Carro carro)
        {
            Carro = carro ?? throw new ArgumentNullException(nameof(carro), "O carro não pode ser nulo.");
            CarroId = carro.Id;
        }
       
    }
}
