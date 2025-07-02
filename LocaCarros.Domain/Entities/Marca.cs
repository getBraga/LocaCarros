
namespace LocaCarros.Domain.Entities
{
    public sealed class Marca
    {
        public int Id { get; private set; }
        public string Nome { get; private set; } = null!;


        private Marca() { }
        public Marca(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome)) { throw new ArgumentException("Precisa Conter um nome válido", nameof(nome)); }
            SetNome(nome);
        }
        public Marca(int id, string nome)
        {
            if (id < 0) { throw new ArgumentException("Id inválido", nameof(id)); }
            if (string.IsNullOrWhiteSpace(nome)) { throw new ArgumentException("Precisa Conter um nome válido", nameof(nome)); }
            Id = id;
            SetNome(nome);
        }

        public void SetNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("O nome da marca é obrigatório.", nameof(nome));

            Nome = nome.Trim();
        }
    }
}
