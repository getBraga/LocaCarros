using AutoMapper;
using LocaCarros.Application.DTOs.ModelosDtos;
using LocaCarros.Application.Services;
using LocaCarros.Domain.Entities;
using LocaCarros.Domain.Enuns;
using LocaCarros.Domain.Exceptions;
using LocaCarros.Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.Application.Tests
{
    public class ModeloServiceTest
    {
        private readonly Mock<IModeloRepository> _modeloRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ModeloService _modeloService;
        private readonly Mock<ICarroRepository> _carroRepositoryMock;

        public ModeloServiceTest()
        {
            _modeloRepositoryMock = new Mock<IModeloRepository>();
            _carroRepositoryMock = new Mock<ICarroRepository>();
            _mapperMock = new Mock<IMapper>();
            _modeloService = new ModeloService(_modeloRepositoryMock.Object, _mapperMock.Object, _carroRepositoryMock.Object);
        }

        private readonly ModeloDTO _modeloDto = new()
        {
            Id = 1,
            MarcaId = 1,
            MarcaNome = "Marca A",
            Motorizacao = 1.0m,
            Nome = "Modelo A",
            TipoCarroceria = Domain.Enuns.EnumTipoCarroceria.Sedan,
            Versao = "Versão A"
        };
        private Modelo _modelo = new("Modelo A", "Versão A", 1.0m, Domain.Enuns.EnumTipoCarroceria.Sedan, new Marca("Marca A"));


        [Fact]
        public async Task GetModelosAsync_DeveRetornarListaDeModelos()
        {

            var modelos = new List<Modelo> { _modelo };

            _modeloRepositoryMock
                .Setup(r => r.GetModelosAsync())
                .ReturnsAsync(modelos);

            _mapperMock
            .Setup(m => m.Map<IEnumerable<ModeloDTO>>(It.IsAny<object>()))
            .Returns([_modeloDto]);

            var result = await _modeloService.GetModelosAsync();

            Assert.IsAssignableFrom<IEnumerable<ModeloDTO>>(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetModelosAsync_DeveRetornarListaVaziaQuandoNaoExistirModelos()
        {
            _modeloRepositoryMock
                .Setup(r => r.GetModelosAsync())
                .ReturnsAsync(new List<Modelo>());
            var result = await _modeloService.GetModelosAsync();
            Assert.Empty(result);
        }
        [Fact]
        public async Task GetByIdEntidadeAsync_DeveRetornarModelo()
        {
            _modeloRepositoryMock
                .Setup(r => r.GetModeloByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_modelo);
            _mapperMock
                .Setup(m => m.Map<Modelo>(It.IsAny<Modelo>()))
                .Returns(_modelo);
            var result = await _modeloService.GetByIdEntidadeAsync(1);
            Assert.NotNull(result);
            Assert.Equal(_modelo.Id, result.Id);
        }
        [Fact]
        public async Task GetByIdEntidadeAsync_DeveRetornarNullQuandoModeloNaoExistir()
        {
            _modeloRepositoryMock
                .Setup(r => r.GetModeloByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Modelo)null!);
            var result = await _modeloService.GetByIdEntidadeAsync(1);
            Assert.Null(result);
        }
        [Fact]
        public async Task GetByIdAsync_DeveRetornarModeloPorId()
        {
            _modeloRepositoryMock
                .Setup(r => r.GetModeloByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_modelo);
            _mapperMock
                .Setup(m => m.Map<ModeloDTO>(It.IsAny<Modelo>()))
                .Returns(_modeloDto);
            var result = await _modeloService.GetByIdAsync(1);
            Assert.NotNull(result);
            Assert.Equal(_modeloDto.Id, result.Id);
        }
        [Fact]
        public async Task GetByIdAsync_DeveRetornaNullQuandoModeloNaoExistir()

        {
            _modeloRepositoryMock
                .Setup(r => r.GetModeloByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Modelo)null!);
            var result = await _modeloService.GetByIdAsync(1);
            Assert.Null(result);

        }
        [Fact]
        public async Task AddAsync_DeveAdicionarModelo()
        {
            var modeloDtoAdd = new ModeloDTOAdd
            {
                Nome = "Modelo A",
                Versao = "Versão A",
                Motorizacao = 1.0m,
                TipoCarroceria = Domain.Enuns.EnumTipoCarroceria.Sedan,
                MarcaId = 1
            };
            _mapperMock
                .Setup(m => m.Map<Modelo>(modeloDtoAdd))
                .Returns(_modelo);
            _modeloRepositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<Modelo>()))
                .ReturnsAsync(_modelo);
            _mapperMock
                .Setup(m => m.Map<ModeloDTO>(It.IsAny<Modelo>()))
                .Returns(_modeloDto);
            var result = await _modeloService.AddAsync(modeloDtoAdd);
            Assert.NotNull(result);
            Assert.Equal(_modeloDto.Id, result.Id);
        }
        [Fact]
        public async Task RemoveAsync_DeveRemoverModeloPorId()
        {
            _modeloRepositoryMock
                .Setup(r => r.GetModeloByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_modelo);
            _modeloRepositoryMock
                .Setup(r => r.DeleteAsync(It.IsAny<Modelo>()))
                .ReturnsAsync(true);
            var result = await _modeloService.RemoveAsync(1);
            Assert.True(result);
        }

        [Fact]
        public async Task RemoveAsync_DeveRetornarFalseQuandoModeloNaoExistir()
        {
            _modeloRepositoryMock
                .Setup(r => r.GetModeloByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Modelo)null!);
            var result = await _modeloService.RemoveAsync(1);
            Assert.False(result);
        }
        [Fact]
        public async Task RemoveAsync_DeveLancarExcecaoQuandoModeloPossuirCarrosAssociados()
        {

            var carro = new Carro("ALJ1155", 2025, "Vermelho", DateTime.Now, EnumCarroStatus.Disponivel, _modelo);

            _modeloRepositoryMock
                .Setup(r => r.GetModeloByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_modelo);
            _carroRepositoryMock
                .Setup(r => r.GetCarrosByModeloIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<Carro> { carro });

            await Assert.ThrowsAsync<DomainException>(() => _modeloService.RemoveAsync(1));


        }

        [Fact]
        public async Task AddAsync_DeveLancarExcecaoQuandoModeloJaExistir()
        {
            var modeloDtoAdd = new ModeloDTOAdd
            {
                Nome = "Modelo A",
                Versao = "Versão A",
                Motorizacao = 1.0m,
                TipoCarroceria = Domain.Enuns.EnumTipoCarroceria.Sedan,
                MarcaId = 1
            };
            _mapperMock
                .Setup(m => m.Map<Modelo>(modeloDtoAdd))
                .Returns(_modelo);
            _modeloRepositoryMock.Setup(_modeloRepositoryMock => _modeloRepositoryMock.GetModeloByNomeAsync(_modelo.Nome))
                .ReturnsAsync(_modelo);
            _modeloRepositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<Modelo>()))
                .ThrowsAsync(new DomainException("Modelo já existe."));
            await Assert.ThrowsAsync<DomainException>(() => _modeloService.AddAsync(modeloDtoAdd));
        }

        [Fact]
        public async Task AddAsync_DeveLancarExcecaoQuandoErroAoCriarModelo()
        {
            var modeloDtoAdd = new ModeloDTOAdd
            {
                Nome = "Modelo A",
                Versao = "Versão A",
                Motorizacao = 1.0m,
                TipoCarroceria = Domain.Enuns.EnumTipoCarroceria.Sedan,
                MarcaId = 1
            };
            _mapperMock
                .Setup(m => m.Map<Modelo>(modeloDtoAdd))
                .Returns(_modelo);
            _modeloRepositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<Modelo>()))
                .ReturnsAsync((Modelo)null!);
            await Assert.ThrowsAsync<DomainException>(() => _modeloService.AddAsync(modeloDtoAdd));
        }
        [Fact]
        public async Task UpdateAsync_DeveAtualizarModelo()
        {
            var modeloDtoUpdate = new ModeloDTOUpdate
            {
                Id = 1,
                Nome = "Modelo Atualizado",
                Versao = "Versão Atualizada",
                Motorizacao = 1.5m,
                TipoCarroceria = Domain.Enuns.EnumTipoCarroceria.SUV,
                MarcaId = 1
            };
            _mapperMock
                .Setup(m => m.Map<Modelo>(modeloDtoUpdate))
                .Returns(_modelo);
            _modeloRepositoryMock
             .Setup(r => r.GetModeloByIdAsync(modeloDtoUpdate.Id))
             .ReturnsAsync(_modelo);
            _modeloRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Modelo>()))
                .ReturnsAsync(_modelo);
            _mapperMock
                .Setup(m => m.Map<ModeloDTO>(It.IsAny<Modelo>()))
                .Returns(_modeloDto);
            var result = await _modeloService.UpdateAsync(modeloDtoUpdate);
            Assert.NotNull(result);
            Assert.Equal(_modeloDto.Id, result.Id);
        }
        [Fact]
        public async Task UpdateAsync_DeveLancarExcecaoQuandoModeloNaoExistir()
        {
            var modeloDtoUpdate = new ModeloDTOUpdate
            {
                Id = 1,
                Nome = "Modelo Atualizado",
                Versao = "Versão Atualizada",
                Motorizacao = 1.5m,
                TipoCarroceria = Domain.Enuns.EnumTipoCarroceria.SUV,
                MarcaId = 1
            };
            _mapperMock
                .Setup(m => m.Map<Modelo>(modeloDtoUpdate))
                .Returns(_modelo);
            _modeloRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Modelo>()))
                .ReturnsAsync((Modelo)null!);
            var ex = await Assert.ThrowsAsync<DomainException>(() => _modeloService.UpdateAsync(modeloDtoUpdate));
            Assert.Equal("Modelo não encontrado.", ex.Message);
        }
        [Fact]
        public async Task UpdateAsync_DeveLancarExcecaoQuandoTentarAtualizarModeloParaUmNomeExistente()
        {
            var modeloDtoUpdate = new ModeloDTOUpdate
            {
                Id = 1,
                Nome = "Modelo Atualizado",
                Versao = "Versão Atualizada",
                Motorizacao = 1.5m,
                TipoCarroceria = Domain.Enuns.EnumTipoCarroceria.SUV,
                MarcaId = 1
            };

            _mapperMock
                .Setup(m => m.Map<Modelo>(modeloDtoUpdate))
                .Returns(_modelo);
            _modeloRepositoryMock
                .Setup(r => r.GetModeloByIdAsync(modeloDtoUpdate.Id))
                .ReturnsAsync(_modelo);
            _modeloRepositoryMock
              .Setup(r => r.GetModeloByNomeAsync(modeloDtoUpdate.Nome))
              .ReturnsAsync(_modelo);

            var ex  = await Assert.ThrowsAsync<DomainException>(() => _modeloService.UpdateAsync(modeloDtoUpdate));
            Assert.Equal("Já existe um modelo com o nome informado.", ex.Message);
        }
    }
}
