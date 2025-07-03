using FluentAssertions;
using LocaCarros.Domain.Entities;
using LocaCarros.Domain.Enuns;

namespace LocaCarros.Domain.Tests;


public class VendaUnitTest
{
    [Fact]
    public void CriarVenda_ComParametrosValidos()
    {
        Modelo modelo = new("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, new Marca("Volkswagen"));
        Carro carro = new("FAX123", 2026, "Vermelho", DateTime.Now, EnumCarroStatus.Disponivel, modelo);
        
        Action action = () =>
        {
            Venda venda = new(10000m, DateTime.Now, carro);
        };
        
        action.Should().NotThrow<ArgumentException>();
    }
    [Fact]
    public void CriarVenda_ComValorNegativo_DeveLancarExcecao()
    {
        Modelo modelo = new("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, new Marca("Volkswagen"));
        Carro carro = new("FAX123", 2026, "Vermelho", DateTime.Now, EnumCarroStatus.Disponivel, modelo);
        
        Action action = () =>
        {
            Venda venda = new(-1000m, DateTime.Now, carro);
        };
        
        action.Should().Throw<ArgumentException>().WithMessage("O valor da venda deve ser maior que zero.*").And.ParamName.Should().Be("valorVenda");
    }
    [Fact]
    public void CriarVenda_ComDataInvalida_DeveLancarExcecao()
    {
        Modelo modelo = new("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, new Marca("Volkswagen"));
        Carro carro = new("FAX123", 2026, "Vermelho", DateTime.Now, EnumCarroStatus.Disponivel, modelo);
        
        Action action = () =>
        {
            Venda venda = new(10000m, default, carro);
        };
        
        action.Should().Throw<ArgumentException>().WithMessage("Data de venda inválida.*").And.ParamName.Should().Be("dataVenda");
    }

    [Fact]
    public void CriarVenda_ComDataFutura_DeveLancarExcecao()
    {
        Modelo modelo = new("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, new Marca("Volkswagen"));
        Carro carro = new("FAX123", 2026, "Vermelho", DateTime.Now, EnumCarroStatus.Disponivel, modelo);
        
        Action action = () =>
        {
            Venda venda = new(10000m, DateTime.Now.AddDays(1), carro);
        };

        action.Should()
            .Throw<ArgumentException>()
            .WithMessage("A data da venda não pode ser no futuro.*")
            .And.ParamName.Should().Be("dataVenda");
    }
    [Fact]
    public void CriarVenda_ComCarroNulo_DeveLancarExcecao()
    {
        Action action = () =>
        {
            Venda venda = new(10000m, DateTime.Now, null!);
        };
        
        action.Should().Throw<ArgumentNullException>().WithMessage("O carro não pode ser nulo.*").And.ParamName.Should().Be("carro");
    }
    [Fact]
    public void AtualizarVenda_ComParametrosValidos()
    {
        Modelo modelo = new("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, new Marca("Volkswagen"));
        Carro carro = new("FAX123", 2026, "Vermelho", DateTime.Now, EnumCarroStatus.Disponivel, modelo);
        Venda venda = new(10000m, DateTime.Now, carro);
        
        Action action = () =>
        {
            venda.Update(12000m, DateTime.Now.AddDays(-1), carro);
        };
        
        action.Should().NotThrow<InvalidOperationException>();
    }
    [Fact]
    public void AtualizarVenda_ComIdInvalido_DeveLancarExcecao()
    {
        Modelo modelo = new("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, new Marca("Volkswagen"));
        Carro carro = new("FAX123", 2026, "Vermelho", DateTime.Now, EnumCarroStatus.Disponivel, modelo);
        Venda venda = new(10000m, DateTime.Now, carro);
        
        venda.GetType().GetProperty("Id")!.SetValue(venda, -1);
        
        Action action = () =>
        {
            venda.Update(12000m, DateTime.Now.AddDays(-1), carro);
        };
        
        action.Should().Throw<InvalidOperationException>().WithMessage("A Venda deve conter um Id válido.");
    }

}
