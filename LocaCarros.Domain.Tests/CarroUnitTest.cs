using FluentAssertions;
using LocaCarros.Domain.Entities;
using LocaCarros.Domain.Enuns;
using LocaCarros.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.Domain.Tests
{
    public class CarroUnitTest
    {
        [Fact]
        public void CriarCarro_ComParametrosValidos()
        {
            Modelo modelo = new("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, new Marca("Volkswagen"));
            Action action = () =>
            {
                var carro = new Carro("FAX123", 2026, "Vermelho", DateTime.Now, EnumCarroStatus.Disponivel, modelo);
            };
            action.Should().NotThrow<ArgumentException>();
        }

        [Fact]
        public void CriarCarro_ComPlacaVazia_DeveLancarExcecao()
        {
            Modelo modelo = new("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, new Marca("Volkswagen"));
            Action action = () =>
            {
                var carro = new Carro("", 2026, "Vermelho", DateTime.Now, EnumCarroStatus.Disponivel, modelo);
            };
            action.Should().Throw<ArgumentException>().WithMessage("A placa não pode ser vazia ou nula.*").And.ParamName.Should().Be("placa");
        }
        [Fact]
        public void CriarCarro_ComPlacaNula_DeveLancarExcecao()
        {
            Modelo modelo = new("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, new Marca("Volkswagen"));
            Action action = () =>
            {
                var carro = new Carro(null!, 2026, "Vermelho", DateTime.Now, EnumCarroStatus.Disponivel, modelo);
            };
            action.Should().Throw<ArgumentException>().WithMessage("A placa não pode ser vazia ou nula.*").And.ParamName.Should().Be("placa");
        }
        [Fact]
        public void DeveCriarCarro_ComStatusDiferenteDeDisponivelParaAluguel_DeveCriar()
        {
            Modelo modelo = new("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, new Marca("Volkswagen"));
            Action action = () =>
            {
                var carro = new Carro("FAX123", 2026, "Vermelho", DateTime.Now, EnumCarroStatus.Disponivel, modelo);
                carro.ValidarDisponibilidadeParaAluguel();
            };
            action.Should().NotThrow<DomainException>();
                
        }
        [Fact]
        public void DeveCriarCarro_ComStatusDiferenteDeDisponivelParaAluguel_DeveLancarExcessao()
        {
            Modelo modelo = new("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, new Marca("Volkswagen"));
            Action action = () =>
            {
                var carro = new Carro("FAX123", 2026, "Vermelho", DateTime.Now, EnumCarroStatus.Alugado, modelo);
                carro.ValidarDisponibilidadeParaAluguel();
            };
            action.Should().Throw<DomainException>().Where(ex => ex.Message.Contains("não está disponível para aluguel!"));
                
        }
        [Fact]
        public void DeveCriarCarro_ComStatusDiferenteDeDisponivelParaVenda_DeveCriar()
        {
            Modelo modelo = new("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, new Marca("Volkswagen"));
            Action action = () =>
            {
                var carro = new Carro("FAX123", 2026, "Vermelho", DateTime.Now, EnumCarroStatus.Disponivel, modelo);
                carro.ValidarDisponibilidadeParaVenda();
            };
            action.Should().NotThrow<DomainException>();
                     
        }
        [Fact]
        public void DeveCriarCarro_ComStatusDiferenteDeDisponivelParaVenda_DeveLancarExcessao()
        {
            Modelo modelo = new("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, new Marca("Volkswagen"));
            Action action = () =>
            {
                var carro = new Carro("FAX123", 2026, "Vermelho", DateTime.Now, EnumCarroStatus.Vendido, modelo);
                carro.ValidarDisponibilidadeParaVenda();
            };
            action.Should().Throw<DomainException>()
                      .Where(ex => ex.Message.Contains("não está disponível para venda"));
        }
        [Fact]
        public void CriarCarro_ComAnoInvalido_DeveLancarExcecao()
        {
            Modelo modelo = new("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, new Marca("Volkswagen"));
            Action action = () =>
            {
                var carro = new Carro("FAX123", 1800, "Vermelho", DateTime.Now, EnumCarroStatus.Disponivel, modelo);
            };
            action.Should().Throw<ArgumentException>().WithMessage("Ano inválido.*").And.ParamName.Should().Be("ano");
        }

        [Fact]
        public void CriarCarro_ComCorVazia_DeveLancarExcecao()
        {
            Modelo modelo = new("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, new Marca("Volkswagen"));
            Action action = () =>
            {
                var carro = new Carro("FAX123", 2026, "", DateTime.Now, EnumCarroStatus.Disponivel, modelo);
            };
            action.Should().Throw<ArgumentException>().WithMessage("A cor não pode ser vazia ou nula.*").And.ParamName.Should().Be("cor");
        }
        [Fact]
        public void CriarCarro_ComDataFabricacaoDefault_DeveLancarExcecao()
        {
            Modelo modelo = new("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, new Marca("Volkswagen"));

            Action action = () =>
            {
                var carro = new Carro("FAX123", 2026, "Vermelho", default, EnumCarroStatus.Disponivel, modelo);
            };
            action.Should().Throw<ArgumentException>().WithMessage("Data de fabricação inválida.*").And.ParamName.Should().Be("dataFabricacao");
        }

        [Fact]
        public void CriarCarro_ComDataFabricacaoFutura_DeveLancarExcecao()
        {
            Modelo modelo = new("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, new Marca("Volkswagen"));
            Action action = () =>
            {
                var carro = new Carro("FAX123", 2026, "Vermelho", DateTime.Now.AddYears(2), EnumCarroStatus.Disponivel, modelo);
            };
            action.Should().Throw<ArgumentException>().WithMessage("A data de fabricação não pode ser no futuro.*").And.ParamName.Should().Be("dataFabricacao");
        }

        [Fact]
        public void CriarCarro_ComStatusInvalido_DeveLancarExcecao()
        {
            Modelo modelo = new("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, new Marca("Volkswagen"));

            int valorInvalido = 999; // Valor que não existe no Enum
            EnumCarroStatus statusInvalido = (EnumCarroStatus)valorInvalido;

            Action act = () =>
            {
                Carro carro = new("XYZ123", 2023, "Azul", DateTime.Now, statusInvalido, modelo);
            };

            act.Should()
                .Throw<ArgumentException>()
                .WithMessage("Status inválido.*")
                .And.ParamName.Should().Be("status");
        }

        [Fact]
        public void CriarCarro_ComModeloNulo_DeveLancarExcecao()
        {
            Action action = () =>
            {
                var carro = new Carro("FAX123", 2026, "Vermelho", DateTime.Now, EnumCarroStatus.Disponivel, null!);
            };
            action.Should().Throw<ArgumentNullException>().WithMessage("O modelo não pode ser nulo.*").And.ParamName.Should().Be("modelo");
        }
        [Fact]
        public void AtualizarCarro_ComParametrosValidos()
        {
            Modelo modelo = new("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, new Marca("Volkswagen"));
            var carro = new Carro("FAX123", 2026, "Vermelho", DateTime.Now, EnumCarroStatus.Disponivel, modelo);
            
            Action action = () =>
            {
                carro.Update("FAX456", 2025, "Azul", DateTime.Now.AddYears(-1), EnumCarroStatus.Alugado, modelo);
            };
            action.Should().NotThrow<InvalidOperationException>();
        }

        [Fact]
        public void AtualizarCarro_ComIdInvalido_DeveLancarExcecao()
        {
            Modelo modelo = new("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, new Marca("Volkswagen"));
            var carro = new Carro("FAX123", 2026, "Vermelho", DateTime.Now, EnumCarroStatus.Disponivel, modelo);
            
            carro.GetType().GetProperty("Id")!.SetValue(carro, -1);
            
            Action action = () =>
            {
                carro.Update("FAX456", 2025, "Azul", DateTime.Now.AddYears(-1), EnumCarroStatus.Vendido, modelo);
            };
            action.Should().Throw<InvalidOperationException>().WithMessage("O Carro deve conter um Id válido.");
        }

    
    }
}
