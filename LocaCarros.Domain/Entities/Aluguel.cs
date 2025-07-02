using System;
using System.Collections.Generic;
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
        public DateTime DataFim { get; private set; }

        public int CarroId { get; private set; }
        public Carro Carro { get; private set; } = null!;

        private Aluguel() { }
        public Aluguel(int id, decimal valorAluguel, DateTime dataInicio, DateTime dataFim, Carro carro)
        {
            SetId(id);
            SetValorAluguel(valorAluguel);
            SetDataFim(dataFim);
            SetDataInicio(dataInicio);
            SetCarro(carro);

        }
        public Aluguel(decimal valorAluguel, DateTime dataInicio, DateTime dataFim, Carro carro)
        {
            SetValorAluguel(valorAluguel);
            SetDataFim(dataFim);
            SetDataInicio(dataInicio);
            SetCarro(carro);

        }
        public void Update(decimal valorAluguel, DateTime dataInicio, DateTime dataFim, Carro carro)
        {
          
            SetValorAluguel(valorAluguel);
            SetDataFim(dataFim);
            SetDataInicio(dataInicio);
            SetCarro(carro);
        }
        private void SetId(int id)
        {
            if (id < 0)
                throw new ArgumentException("Id precisa ser maior ou igual a zero.", nameof(id));
            Id = id;
        }

        private void SetValorAluguel(decimal valorAluguel)
        {
            if (valorAluguel <= 0)
                throw new ArgumentException("Valor do aluguel deve ser maior que zero.", nameof(valorAluguel));
            ValorAluguel = valorAluguel;
        }
        private void SetDataInicio(DateTime dataInicio)
        {
            if (dataInicio == default)
                throw new ArgumentException("Data de início inválida.", nameof(dataInicio));
            DataInicio = dataInicio;
        }
        private void SetDataFim(DateTime dataFim)
        {
            if (dataFim == default)
                throw new ArgumentException("Data de fim inválida.", nameof(dataFim));
            if (dataFim <= DataInicio)
                throw new ArgumentException("Data de fim deve ser posterior à data de início.", nameof(dataFim));
            DataFim = dataFim;
        }
        private void SetCarro(Carro carro)
        {
            Carro = carro ?? throw new ArgumentNullException(nameof(carro), "Carro não pode ser nulo.");
            CarroId = carro.Id;
        }
       
    }
}
