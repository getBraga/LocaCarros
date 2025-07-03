using LocaCarros.Domain.Enuns;
using System.Diagnostics.CodeAnalysis;


namespace LocaCarros.Domain.Entities
{
    public sealed class Modelo
    {
        public int Id { get; private set; }
        public string Nome { get; private set; } = string.Empty;
        public string Versao { get; private set; } = string.Empty;
        public decimal Motorizacao { get; private set; }
        public EnumTipoCarroceria TipoCarroceria { get; private set; }
        public int MarcaId { get; private set; }
        public Marca Marca { get; private set; } = null!;

        [ExcludeFromCodeCoverage]
        private Modelo() { }
   
        public Modelo(string nome, string versao, decimal motorizacao, EnumTipoCarroceria tipoCarroceria, Marca marca)
        {
            SetNome(nome);
            SetVersao(versao);
            SetMotorizacao(motorizacao);
            SetTipoCarroceria(tipoCarroceria);
            SetMarca(marca);
        }

  
        private void SetNome(string nome)
        {
            if (String.IsNullOrWhiteSpace(nome))
            {
                throw new ArgumentException("Precisa ter um nome válido", nameof(nome));
            }
            Nome = nome.Trim();
        }
        private void SetVersao(string versao)
        {
            if (String.IsNullOrEmpty(versao))
                throw new ArgumentException("A versão precisa ter um valor válido", nameof(versao));
            Versao = versao.Trim();
        }
        private void SetMotorizacao(decimal motorizacao)
        {
            if (motorizacao <= 0)
                throw new ArgumentException("A motorização precisa conter um valor válido", nameof(motorizacao));
            Motorizacao = motorizacao;
        }

        private void SetTipoCarroceria(EnumTipoCarroceria tipoCarroceria)
        {
            if (!Enum.IsDefined(tipoCarroceria))
            {
                throw new ArgumentException("Tipo de carroceria inválido.", nameof(tipoCarroceria));
            }
            TipoCarroceria = tipoCarroceria;
        }

        private void SetMarca(Marca marca)
        {
        
            Marca = marca ?? throw new ArgumentNullException(nameof(marca), "A marca não pode ser nula.");
            MarcaId = marca.Id;
        }

        public void Update(string nome, string versao, decimal motorizacao, EnumTipoCarroceria tipoCarroceria, Marca marca)
        {
            if (Id < 0)
                throw new InvalidOperationException("O Modelo deve conter um Id válido.");
            SetNome(nome);
            SetVersao(versao);
            SetMotorizacao(motorizacao);
            SetTipoCarroceria(tipoCarroceria);
            SetMarca(marca);

        }
    }
}
