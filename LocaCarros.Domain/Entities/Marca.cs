
using LocaCarros.Domain.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace LocaCarros.Domain.Entities
{
    public sealed class Marca
    {
        public int Id { get; private set; }
        public string Nome { get; private set; } = null!;

        [ExcludeFromCodeCoverage]
        private Marca() { }
        public Marca(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome)) { throw new ArgumentException("Precisa Conter um nome válido", nameof(nome)); }
            SetNome(nome);
        }
        public void ValidarRemover(bool modelosMarca)
        {

            if (modelosMarca)
                throw new DomainException("Não é possível excluir a marca, pois existem modelos associados a ela.");
        }
        public void Update(string nome)
        {
            if (Id < 0)
                throw new InvalidOperationException("A marca deve conter um Id válido.");
            SetNome(nome);
        }
        private void SetNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("O nome da marca é obrigatório.", nameof(nome));

            Nome = nome.Trim();
        }

    }
}
