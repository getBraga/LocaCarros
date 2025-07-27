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
    public class ModeloUnitTest
    {
        [Fact]
        public void DeveCriarModelo_ComParametros_Validos()
        {

            var nome = "Fusca";
            var versao = "2023";
            var motorizacao = 1.0m;
            var tipoCarroceria = EnumTipoCarroceria.Hatch;

            var marca = new Marca("Volkswagen");

            var modelo = new Modelo(nome, versao, motorizacao, tipoCarroceria, marca);

            Assert.NotNull(modelo);
            Assert.Equal(nome, modelo.Nome);
            Assert.Equal(versao, modelo.Versao);
            Assert.Equal(motorizacao, modelo.Motorizacao);
            Assert.Equal(tipoCarroceria, modelo.TipoCarroceria);
            Assert.Equal(marca, modelo.Marca);
        }
        [Fact]
        public void DeveLancarExcecao_QuandoNomeInvalido()
        {
            var marca = new Marca("Volkswagen");
            Action action = () =>
            {
                Modelo modelo = new("", "2023", 1.0m, EnumTipoCarroceria.Hatch, marca);
            };
            action.Should().Throw<ArgumentException>()
                .WithMessage("Precisa ter um nome válido*")
                .And.ParamName.Should().Be("nome");
        }

        [Fact]
        public void DeveLancarExcecao_QuandoVersaoInvalida()
        {
            var marca = new Marca("Volkswagen");
            Action action = () =>
            {
                Modelo modelo = new("Fusca", "", 1.0m, EnumTipoCarroceria.Hatch, marca);
            };
            action.Should().Throw<ArgumentException>()
                .WithMessage("A versão precisa ter um valor válido*")
                .And.ParamName.Should().Be("versao");
        }
        [Fact]
        public void DeveLancarExcecao_QuandoMotorizacaoInvalida()
        {
            var marca = new Marca("Chevrolet");
            Action action = () =>
            {
                Modelo modelo = new("Fusca", "2020", 0, EnumTipoCarroceria.Picape, marca);
            };
            action.Should().Throw<ArgumentException>()
           .WithMessage("A motorização precisa conter um valor válido*")
             .And.ParamName.Should().Be("motorizacao");
        }
        [Fact]
        public void DeveLancarExcecao_QuandoTipoCarroceriaInvalido()
        {
            var marca = new Marca("Chevrolet");
            Action action = () =>
            {
                Modelo modelo = new("Fusca", "2020", 1.0m, (EnumTipoCarroceria)999, marca);
            };
            action.Should().Throw<ArgumentException>()
                .WithMessage("Tipo de carroceria inválido.*")
                .And.ParamName.Should().Be("tipoCarroceria");
        }
        [Fact]
        public void DeveLancarExcecao_QuandoMarcaInvalida()
        {
            Action action = () =>
            {
                Modelo modelo = new("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, null!);
            };
            action.Should().Throw<ArgumentNullException>()
               .WithMessage("A marca não pode ser nula.*")
                .And.ParamName.Should().Be("marca");
        }
        [Fact]
        public void DeveAtualizarModelo_ComParametros_Validos()
        {
            var marca = new Marca("Volkswagen");
            var modelo = new Modelo("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, marca);
            var novoNome = "Gol";
            var novaVersao = "2024";
            var novaMotorizacao = 1.6m;
            var novoTipoCarroceria = EnumTipoCarroceria.Sedan;
            modelo.Update(novoNome, novaVersao, novaMotorizacao, novoTipoCarroceria, marca);
            Assert.Equal(novoNome, modelo.Nome);
            Assert.Equal(novaVersao, modelo.Versao);
            Assert.Equal(novaMotorizacao, modelo.Motorizacao);
            Assert.Equal(novoTipoCarroceria, modelo.TipoCarroceria);
        }

        [Fact]
        public void DeveLancarExcecao_QuandoAtualizarModeloComIdInvalido()
        {
            var marca = new Marca("Volkswagen");
            var modelo = new Modelo("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, marca);
            Action action = () =>
            {
                modelo.GetType().GetProperty("Id")!.SetValue(modelo, -1);
                modelo.Update("Fusca", "2024", 1.6m, EnumTipoCarroceria.Sedan, marca);
            };
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("O Modelo deve conter um Id válido.");

        }
        [Fact]
        public void DeveLancarExcecao_QuandoRemoverModeloComModelosAssociados()
        {
            var marca = new Marca("Volkswagen");
            var modelo = new Modelo("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, marca);
            Action action = () => modelo.ValidarRemover(1);
            action.Should().Throw<DomainException>()
                .WithMessage("Não é possível excluir o modelo, pois existem marcas associadas a ele.");
        }
        [Fact]
        public void DeveRemoverModelo_ValidarSemExcecao()
        {
            var marca = new Marca("Volkswagen");
            var modelo = new Modelo("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, marca);
            Action action = () => modelo.ValidarRemover(0);
            action.Should().NotThrow<DomainException>();
        }
        [Fact]
        public void DeveLancarExcecao_QuandoValidarModeloComMesmoNomePorId()
        {
            var marca = new Marca("Volkswagen");
            var modelo = new Modelo("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, marca);
            Action action = () => modelo.ValidarModeloComMesmoNomePorId(1, 2);
            action.Should().Throw<DomainException>()
                .WithMessage("Já existe um modelo com esse nome.");
        }
        [Fact]
        public void DeveLancarExcecao_QuandoValidarNomeVazio()
        {
            var marca = new Marca("Volkswagen");
            var modelo = new Modelo("Teste", "2023", 1.0m, EnumTipoCarroceria.Hatch, marca);
            Action action = () => modelo.ValidarNome("");
            action.Should().Throw<DomainException>()
                           .WithMessage("Precisa ter um nome válido");
        }
        [Fact]
        public void DeveLancarExcecao_QuandoValidarNomeMenorQue3Caracteres()
        {
            var marca = new Marca("Volkswagen");
            var modelo = new Modelo("Teste", "2023", 1.0m, EnumTipoCarroceria.Hatch, marca);
            Action action = () => modelo.ValidarNome("ab");
            action.Should().Throw<DomainException>()
                           .WithMessage("Precisa ter um nome válido");
        }
        [Fact]
        public void DeveValidar_QuandoCondicoesAtendidas()
        {
            var marca = new Marca("Volkswagen");
            var modelo = new Modelo("Teste", "2023", 1.0m, EnumTipoCarroceria.Hatch, marca);
            Action action = () => modelo.ValidarNome(modelo.Nome);
            action.Should().NotThrow<DomainException>();
                           
        }
        [Fact]
        public void DeveAtualizarModelo_ComMesmoNomePorId_ValidarSemExcecao()
        {
            var marca = new Marca("Volkswagen");
            var modelo = new Modelo("Fusca", "2023", 1.0m, EnumTipoCarroceria.Hatch, marca);
            Action action = () => modelo.ValidarModeloComMesmoNomePorId(1, 1);
            action.Should().NotThrow<DomainException>();
        }
    }
}
