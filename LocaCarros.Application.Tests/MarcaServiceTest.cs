using AutoMapper;
using LocaCarros.Application.DTOs.MarcasDtos;
using LocaCarros.Application.Services;
using LocaCarros.Domain.Entities;
using LocaCarros.Domain.Enuns;
using LocaCarros.Domain.Exceptions;
using LocaCarros.Domain.Interfaces;
using Moq;
using Xunit;

namespace LocaCarros.Application.Tests
{
    public class MarcaServiceTest
    {
        private readonly Mock<IMarcaRepository> _marcaRepositoryMock;
        private readonly Mock<IModeloRepository> _modeloRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly MarcaService _marcaService;

        private readonly Marca _marca = new("Marca A");
        private readonly Modelo _modelo;
        private readonly MarcaDTO _marcaDto = new() { Id = 1, Nome = "Marca A" };

        public MarcaServiceTest()
        {
            _marcaRepositoryMock = new Mock<IMarcaRepository>();
            _modeloRepositoryMock = new Mock<IModeloRepository>();
            _mapperMock = new Mock<IMapper>();
            _marcaService = new MarcaService(_marcaRepositoryMock.Object, _mapperMock.Object, _modeloRepositoryMock.Object);
            _modelo = new Modelo("Modelo A", "1.0", 1.0m, EnumTipoCarroceria.Coupe, _marca);
        }

        [Fact]
        public async Task Get_DeveRetornarListaDeMarcas()
        {
            var marcas = new List<Marca> { new("Marca A"), new("Marca B") };

            _marcaRepositoryMock
                .Setup(r => r.GetMarcasAsync())
                .ReturnsAsync(marcas);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<Marca>, IEnumerable<MarcaDTO>>(It.IsAny<IEnumerable<Marca>>()))
                .Returns([_marcaDto]);

            var result = await _marcaService.GetMarcasAsync();

            Assert.IsAssignableFrom<IEnumerable<MarcaDTO>>(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetById_DeveRetornarMarca()
        {
            _marcaRepositoryMock
                .Setup(r => r.GetMarcaByIdAsync(1))
                .ReturnsAsync(_marca);

            _mapperMock
                .Setup(m => m.Map<Marca, MarcaDTO>(_marca))
                .Returns(_marcaDto);

            var result = await _marcaService.GetByIdAsync(1);

            Assert.IsType<MarcaDTO>(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Marca A", result.Nome);
        }

        [Fact]
        public async Task GetById_DeveRetornarNull()
        {
            _marcaRepositoryMock
                .Setup(r => r.GetMarcaByIdAsync(1))
                .ReturnsAsync((Marca?)null);

            var result = await _marcaService.GetByIdAsync(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task RemoveAsync_DeveRetornarFalse_QuandoMarcaNaoExiste()
        {
            _marcaRepositoryMock
                .Setup(r => r.GetMarcaByIdAsync(1))
                .ReturnsAsync((Marca?)null);

            var result = await _marcaService.RemoveAsync(1);

            Assert.False(result);
        }

        [Fact]
        public async Task RemoveAsync_DeveLancarExcecao_QuandoNaoPodeDeletar()
        {
            _marcaRepositoryMock
                .Setup(r => r.GetMarcaByIdAsync(2))
                .ReturnsAsync(_marca);

            _modeloRepositoryMock
                .Setup(r => r.GetModelosByMarcaIdAsync(2))
                .ReturnsAsync(new List<Modelo> { _modelo });

            var exception = await Assert.ThrowsAsync<DomainException>(() => _marcaService.RemoveAsync(2));
            Assert.Equal("Não é possível excluir marca com modelos.", exception.Message);
        }

        [Fact]
        public async Task RemoveAsync_DeveDeletarMarca()
        {
            _marcaRepositoryMock
                .Setup(r => r.GetMarcaByIdAsync(3))
                .ReturnsAsync(_marca);

            _modeloRepositoryMock
                .Setup(r => r.GetModelosByMarcaIdAsync(It.IsAny<int>()))
                .ReturnsAsync((List<Modelo>?)null!);
          

            _marcaRepositoryMock
                .Setup(r => r.DeleteAsync(_marca))
                .ReturnsAsync(true);

            var result = await _marcaService.RemoveAsync(3);

            Assert.True(result);
            _marcaRepositoryMock.Verify(r => r.DeleteAsync(_marca), Times.Once);
        }

        [Fact]
        public async Task AddAsync_DeveAdicionarMarca()
        {
            var dtoAdd = new MarcaDTOAdd { Nome = "Marca A" };

            _mapperMock
                .Setup(m => m.Map<Marca>(dtoAdd))
                .Returns(_marca);

            _marcaRepositoryMock
                .Setup(r => r.CreateAsync(_marca))
                .ReturnsAsync(_marca);

            _mapperMock
                .Setup(m => m.Map<Marca, MarcaDTO>(_marca))
                .Returns(_marcaDto);

            var result = await _marcaService.AddAsync(dtoAdd);

            Assert.NotNull(result);
            Assert.Equal("Marca A", result.Nome);
        }

        [Fact]
        public async Task UpdateAsync_DeveAtualizarMarca()
        {
            _mapperMock
                .Setup(m => m.Map<MarcaDTO, Marca>(_marcaDto))
                .Returns(_marca);

            _marcaRepositoryMock
                .Setup(r => r.UpdateAsync(_marca))
                .ReturnsAsync(_marca);

            _mapperMock
                .Setup(m => m.Map<Marca, MarcaDTO>(_marca))
                .Returns(_marcaDto);

            var result = await _marcaService.UpdateAsync(_marcaDto);

            Assert.NotNull(result);
            Assert.Equal("Marca A", result.Nome);
        }
    }
}
