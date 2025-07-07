

using LocaCarros.Domain.Enuns;
using System.Diagnostics.CodeAnalysis;

namespace LocaCarros.Domain.Entities
{
    public sealed class Carro
    {
        public int Id { get; private set; }
        public string Placa { get; private set; } = null!;
        public int Ano { get; private set; }
        public string Cor { get; private set; } = null!;
        public DateTime DataFabricacao { get; private set; }
        public EnumCarroStatus Status { get; private set; } = EnumCarroStatus.Disponivel;
        public int ModeloId { get; private set; }
        public Modelo Modelo { get; private set; } = null!;
        public ICollection<Aluguel> Alugueis { get; private set; } = new List<Aluguel>()!;
        [ExcludeFromCodeCoverage]
        private Carro() { }
      

        public Carro(string placa, int ano, string cor, DateTime dataFabricacao, EnumCarroStatus status, Modelo modelo)
        {
            SetPlaca(placa);
            SetAno(ano);
            SetCor(cor);
            SetDataFabricacao(dataFabricacao);
            SetStatus(status);
            SetModelo(modelo);
        }

        public void Update(string placa, int ano, string cor, DateTime dataFabricacao, EnumCarroStatus status, Modelo modelo)
        {
            if (Id < 0)
                throw new InvalidOperationException("O Carro deve conter um Id válido.");
            SetPlaca(placa);
            SetAno(ano);
            SetCor(cor);
            SetDataFabricacao(dataFabricacao);
            SetStatus(status);
            SetModelo(modelo);
        }

   

        private void SetPlaca(string placa)
        {
            if (string.IsNullOrWhiteSpace(placa))
            {
                throw new ArgumentException("A placa não pode ser vazia ou nula.", nameof(placa));
            }
            Placa = placa.Trim().ToUpper();
        }
        private void SetAno(int ano)
        {
            if (ano < 1886 || ano > DateTime.Now.Year + 1) 
            {
                throw new ArgumentException("Ano inválido.", nameof(ano));
            }
            Ano = ano;
        }
        private void SetCor(string cor)
        {
            if (string.IsNullOrWhiteSpace(cor))
            {
                throw new ArgumentException("A cor não pode ser vazia ou nula.", nameof(cor));
            }
            Cor = cor.Trim();
        }
        private void SetDataFabricacao(DateTime dataFabricacao)
        {
            if (dataFabricacao == default)
            {
                throw new ArgumentException("Data de fabricação inválida.", nameof(dataFabricacao));
            }
            if(dataFabricacao > DateTime.Now)
            {
                throw new ArgumentException("A data de fabricação não pode ser no futuro.", nameof(dataFabricacao));
            }
            DataFabricacao = dataFabricacao;
        }
        public void SetStatus(EnumCarroStatus status)
        {
            if (!Enum.IsDefined(status))
            {
                throw new ArgumentException("Status inválido.", nameof(status));
            }
            Status = status;
        }
        private void SetModelo(Modelo modelo)
        {
            Modelo = modelo ?? throw new ArgumentNullException(nameof(modelo), "O modelo não pode ser nulo.");
            ModeloId = modelo.Id;
            
        }
      
    }
}
