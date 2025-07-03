using FluentAssertions;
using LocaCarros.Domain.Entities;

namespace LocaCarros.Domain.Tests
{
    public class MarcaUnitTest
    {
        [Fact]
        public void CriarMarca_ComParametrosValidos()
        {
            
            Action action = () =>
            {
                Marca marca = new("Toyota");
            };
            action.Should().NotThrow<ArgumentException>();
        }
        [Fact]
        public void CriarMarca_ComNomeVazio_DeveLancarExcecao()
        {
            Action action = () =>
            {
                Marca marca = new("");
            };
            action.Should().Throw<ArgumentException>().WithMessage("Precisa Conter um nome válido*").And.ParamName.Should().Be("nome");
        }
        [Fact]
        public void CriarMarca_ComNomeNulo_DeveLancarExcecao()
        {
            Action action = () =>
            {
                Marca marca = new(null!);
            };
            action.Should().Throw<ArgumentException>().WithMessage("Precisa Conter um nome válido*").And.ParamName.Should().Be("nome");
        }
        [Fact]
        public void AtualizarMarca_ComNomeInvalido_DeveLancarExcecao()
        {
            // Arrange
            var marca = new Marca("Toyota");

            // Act
            Action act = () => marca.Update("   ");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("O nome da marca é obrigatório*")
                .And.ParamName.Should().Be("nome");
        }

        [Fact]
        public void AtualizarMarca_ComNomeValido_DeveAtualizarNome()
        {
            Marca marca = new("Toyota");
            marca.Update("Honda");
            marca.Nome.Should().Be("Honda");
        }

        [Fact]
        public void AtualizarMarca_ComIdInvalido_DeveLancarExcecao()
        {
            
            var marca = new Marca("Toyota");
         
            marca.GetType().GetProperty("Id")!.SetValue(marca, -1);
           
            Action act = () => marca.Update("Honda");
          
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("A marca deve conter um Id válido.");
        }
    }
}
