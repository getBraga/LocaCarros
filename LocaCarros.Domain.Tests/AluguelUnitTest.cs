using FluentAssertions;
using LocaCarros.Domain.Entities;
using LocaCarros.Domain.Enuns;


namespace LocaCarros.Domain.Tests
{
    public class AluguelUnitTest
    {
        [Fact]
        public void CriarAluguel_ComParametrosValidos()
        {
            Modelo modelo = new("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, new Marca("Volkswagen"));
            Carro carro = new("FAX123", 2026, "Vermelho", DateTime.Now, EnumCarroStatus.Disponivel, modelo);
            Action action = () =>
            {
                Aluguel aluguel = new(100m, DateTime.Now, DateTime.Now.AddDays(1), carro);
            };
            action.Should().NotThrow<ArgumentException>();
        }
        [Fact]
        public void CriarAluguel_ComValorNegativo_DeveLancarExcecao()
        {
            Modelo modelo = new("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, new Marca("Volkswagen"));
            Carro carro = new("FAX123", 2026, "Vermelho", DateTime.Now, EnumCarroStatus.Disponivel, modelo);
            Action action = () =>
            {
                Aluguel aluguel = new(-100m, DateTime.Now, DateTime.Now.AddDays(1), carro);
            };
            action.Should().Throw<ArgumentException>().WithMessage("O valor do aluguel deve ser maior que zero.*").And.ParamName.Should().Be("valorAluguel");
        }
        [Fact]
        public void CriarAluguel_ComDataInicioInvalida_DeveLancarExcecao()
        {
            Modelo modelo = new("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, new Marca("Volkswagen"));
            Carro carro = new("FAX123", 2026, "Vermelho", DateTime.Now, EnumCarroStatus.Disponivel, modelo);
            Action action = () =>
            {
                Aluguel aluguel = new(100m, default, DateTime.Now.AddDays(1), carro);
            };
            action.Should().Throw<ArgumentException>().WithMessage("Data de início inválida.*").And.ParamName.Should().Be("dataInicio");
        }

        [Fact]
        public void CriarAluguel_ComDataDevolucaoInvalida_DeveLancarExcecao()
        {
            Modelo modelo = new("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, new Marca("Volkswagen"));
            Carro carro = new("FAX123", 2026, "Vermelho", DateTime.Now, EnumCarroStatus.Disponivel, modelo);
            Action action = () =>
            {
                Aluguel aluguel = new(100m, DateTime.Now.AddDays(1), default, carro);
            };
            action.Should().Throw<ArgumentException>().WithMessage("Data de devolucao é inválida.*").And.ParamName.Should().Be("dataDevolucao");
        }
        [Fact]
        public void CriarAluguel_ComDataDevolucaoAnterior_DeveLancarExcecao()
        {
            Modelo modelo = new("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, new Marca("Volkswagen"));
            Carro carro = new("FAX123", 2026, "Vermelho", DateTime.Now, EnumCarroStatus.Disponivel, modelo);
            Action action = () =>
            {
                Aluguel aluguel = new(100m, DateTime.Now.AddYears(2), DateTime.Now, carro);
            };
            action.Should().Throw<ArgumentException>().WithMessage("A data de devolução não pode ser anterior à data de início do aluguel.*").And.ParamName.Should().Be("dataDevolucao");
        }
        [Fact]
        public void CriarAluguel_ComCarroNulo_DeveLancarExcecao()
        {
            Action action = () =>
            {
                Aluguel aluguel = new(100m, DateTime.Now, DateTime.Now.AddDays(1), null!);
            };
            action.Should().Throw<ArgumentNullException>().WithMessage("O carro não pode ser nulo.*").And.ParamName.Should().Be("carro");
        }
        [Fact]
        public void AtualizarAluguel_ComParametrosValidos()
        {
            Modelo modelo = new("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, new Marca("Volkswagen"));
            Carro carro = new("FAX123", 2026, "Vermelho", DateTime.Now, EnumCarroStatus.Disponivel, modelo);

            Aluguel aluguel = new(100m, DateTime.Now, DateTime.Now.AddDays(1), carro);
            aluguel.TrocarCarro(carro, DateTime.Now, DateTime.Now.AddDays(1), 123);
            Action action = () =>
            {
                aluguel.Update(150m, DateTime.Now.AddDays(2), DateTime.Now.AddDays(3), carro);
            };
            action.Should().NotThrow<InvalidOperationException>();
        }
        [Fact]
        public void AtualizarAluguel_ComIdInvalido_DeveLancarExcecao()
        {
            Modelo modelo = new("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, new Marca("Volkswagen"));
            Carro carro = new("FAX123", 2026, "Vermelho", DateTime.Now, EnumCarroStatus.Disponivel, modelo);
            Aluguel aluguel = new(100m, DateTime.Now, DateTime.Now.AddDays(1), carro);
            aluguel.GetType().GetProperty("Id")!.SetValue(aluguel, -1);
            Action action = () =>
            {
                aluguel.Update(150m, DateTime.Now.AddDays(2), DateTime.Now.AddDays(3), carro);
            };
            action.Should().Throw<InvalidOperationException>().WithMessage("O Aluguel deve conter um Id válido.");
        }
        
    }
}
