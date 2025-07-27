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
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ModeloService _modeloService;

        public ModeloServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _modeloService = new ModeloService(_unitOfWorkMock.Object, _mapperMock.Object);
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

            _unitOfWorkMock
                .Setup(r => r.Modelos.GetModelosAsync())
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
            _unitOfWorkMock
                .Setup(r => r.Modelos.GetModelosAsync())
                .ReturnsAsync(new List<Modelo>());
            var result = await _modeloService.GetModelosAsync();
            Assert.Empty(result);
        }
        [Fact]
        public async Task GetByMarcaIdAsync_DeveRetornarListaDeModelosPorMarcaId()
        {
            var modelos = new List<Modelo> { _modelo };
            _unitOfWorkMock
                .Setup(r => r.Modelos.GetModelosByMarcaIdAsync(It.IsAny<int>()))
                .ReturnsAsync(modelos);
            _mapperMock
                .Setup(m => m.Map<IEnumerable<ModeloDTO>>(It.IsAny<IEnumerable<Modelo>>()))
                .Returns([_modeloDto]);
            var result = await _modeloService.GetByMarcaIdAsync(1);
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(_modeloDto.Id, result.First().Id);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarModeloPorId()
        {
            _unitOfWorkMock
                .Setup(r => r.Modelos.GetModeloByIdAsync(It.IsAny<int>()))
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
            _unitOfWorkMock
                .Setup(r => r.Modelos.GetModeloByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Modelo)null!);
            var result = await _modeloService.GetByIdAsync(1);
            Assert.Null(result);

        }

        [Fact]
        public async Task RemoveAsync_DeveRemoverModeloPorId()
        {
            _unitOfWorkMock
                .Setup(r => r.Modelos.GetModeloByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_modelo);

            _unitOfWorkMock.Setup(m => m.Carros.GetCarrosByModeloIdAsync(1)).ReturnsAsync(new List<Carro>());
            _unitOfWorkMock
                .Setup(r => r.Modelos.DeleteAsync(It.IsAny<Modelo>()))
                .ReturnsAsync(true);
            var result = await _modeloService.RemoveAsync(1);
            Assert.True(result);
        }

        [Fact]
        public async Task RemoveAsync_DeveRetornarFalseQuandoModeloNaoExistir()
        {
            _unitOfWorkMock
                .Setup(r => r.Modelos.GetModeloByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Modelo)null!);
            var result = await _modeloService.RemoveAsync(1);
            Assert.False(result);
        }
        [Fact]
        public async Task RemoveAsync_DeveLancarExcecaoQuandoModeloPossuirCarrosAssociados()
        {

            var carro = new Carro("ALJ1155", 2025, "Vermelho", DateTime.Now, EnumCarroStatus.Disponivel, _modelo);

            _unitOfWorkMock
                .Setup(r => r.Modelos.GetModeloByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(_modelo);
            _unitOfWorkMock
                .Setup(r => r.Carros.GetCarrosByModeloIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<Carro> { carro });

            await Assert.ThrowsAsync<DomainException>(() => _modeloService.RemoveAsync(1));


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
            _unitOfWorkMock
                .Setup(r => r.Modelos.CreateAsync(It.IsAny<Modelo>()))
                .ReturnsAsync(_modelo);
            _mapperMock
                .Setup(m => m.Map<ModeloDTO>(It.IsAny<Modelo>()))
                .Returns(_modeloDto);
            var result = await _modeloService.AddAsync(modeloDtoAdd);
            Assert.NotNull(result);
            Assert.Equal(_modeloDto.Id, result.Id);
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
            _unitOfWorkMock.Setup(_modeloRepositoryMock => _modeloRepositoryMock.Modelos.GetModeloByNomeAsync(_modelo.Nome))
                .ReturnsAsync(_modelo);
            _unitOfWorkMock
                .Setup(r => r.Modelos.CreateAsync(It.IsAny<Modelo>()))
                .ThrowsAsync(new DomainException("Já existe um modelo com esse nome."));
            var ex = await Assert.ThrowsAsync<DomainException>(() => _modeloService.AddAsync(modeloDtoAdd));
            Assert.Equal("Já existe um modelo com esse nome.", ex.Message);
        }

        [Fact]
        public async Task AddAsync_DeveLancarExcecaoQuandoCriarModelo()
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
            _unitOfWorkMock
                .Setup(r => r.Modelos.CreateAsync(It.IsAny<Modelo>()))
                .ThrowsAsync(new DomainException("Erro ao criar Modelo"));
            var ex = await Assert.ThrowsAsync<DomainException>(() => _modeloService.AddAsync(modeloDtoAdd));
            Assert.Equal("Erro ao criar Modelo", ex.Message);
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
            _unitOfWorkMock
             .Setup(r => r.Modelos.GetModeloByIdAsync(modeloDtoUpdate.Id))
             .ReturnsAsync(_modelo);
            _unitOfWorkMock
                .Setup(r => r.Modelos.UpdateAsync(It.IsAny<Modelo>()))
                .ReturnsAsync(_modelo);
            _mapperMock
                .Setup(m => m.Map<ModeloDTO>(It.IsAny<Modelo>()))
                .Returns(_modeloDto);
            var result = await _modeloService.UpdateAsync(modeloDtoUpdate);
            Assert.NotNull(result);
            Assert.Equal(_modeloDto.Id, result.Id);
        }
        [Fact]
        public async Task UpdateAsync_DeveLancarDomainExcecaoQuandoModeloNaoExistir()
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

            _unitOfWorkMock
                .Setup(r => r.Modelos.UpdateAsync(It.IsAny<Modelo>()))
                .ReturnsAsync((Modelo)null!);
            var ex = await Assert.ThrowsAsync<DomainException>(() => _modeloService.UpdateAsync(modeloDtoUpdate));
            Assert.Equal("Modelo não encontrado.", ex.Message);
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

            _unitOfWorkMock.Setup(m => m.Modelos.GetModeloByIdAsync(modeloDtoUpdate.Id))
                .ReturnsAsync(_modelo);
            _unitOfWorkMock
                .Setup(r => r.Modelos.UpdateAsync(It.IsAny<Modelo>()))
               .ThrowsAsync(new Exception("Erro ao atualizar o modelo."));
            var ex = await Assert.ThrowsAsync<Exception>(() => _modeloService.UpdateAsync(modeloDtoUpdate));
            Assert.Equal("Erro ao atualizar o modelo.", ex.Message);
        }
        [Fact]
        public async Task UpdateAsync_DeveLancarDomainExcecaoQuandoTentarAtualizarModeloParaUmNomeExistente()
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
            _unitOfWorkMock
                .Setup(r => r.Modelos.GetModeloByIdAsync(modeloDtoUpdate.Id))
                .ReturnsAsync(_modelo);
            _unitOfWorkMock
              .Setup(r => r.Modelos.GetModeloByNomeAsync(modeloDtoUpdate.Nome))
              .ReturnsAsync(_modelo);

            var ex = await Assert.ThrowsAsync<DomainException>(() => _modeloService.UpdateAsync(modeloDtoUpdate));
            Assert.Equal("Já existe um modelo com esse nome.", ex.Message);
        }
    }
}
