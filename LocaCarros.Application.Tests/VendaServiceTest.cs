using AutoMapper;
using LocaCarros.Application.DTOs.AlugueisDtos;
using LocaCarros.Application.DTOs.VendasDtos;
using LocaCarros.Application.Services;
using LocaCarros.Domain.Entities;
using LocaCarros.Domain.Enuns;
using LocaCarros.Domain.Exceptions;
using LocaCarros.Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.Application.Tests
{
    public class VendaServiceTest
    {
        private readonly VendaService _vendaService;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Venda _venda;
        private readonly Carro _carro;
        private readonly Modelo _modelo;
        private readonly DateTime _dataVenda = DateTime.Now;
        private readonly VendaDTO _vendaDto;
        private readonly VendaDTOAdd _vendaDTOAdd;
        private readonly VendaDTOUpdate _vendaDTOUpdate; 
        public VendaServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _vendaService = new VendaService(_unitOfWorkMock.Object, _mapperMock.Object);

            _modelo = new Modelo("Modelo Teste", "1.0", 1.0m, EnumTipoCarroceria.Hatch, new Marca("Marca Teste"));
            _carro = new Carro("ABC1234", 2020, "Preto", new DateTime(2020, 1, 1), EnumCarroStatus.Disponivel, _modelo);
            _venda = new Venda(15550.67m, _dataVenda.AddMonths(-1), _carro);
            _vendaDto = new VendaDTO
            {
                Id = _venda.Id,
                ValorVenda = _venda.ValorVenda,
                DataVenda = _venda.DataVenda.ToString("dd/MM/yyyy"),
                CarroAno = _carro.Ano,
                CarroCor = _carro.Cor,
                CarroDataFabricacao = _carro.DataFabricacao.ToString("dd/MM/yyyy"),
                CarroPlaca = _carro.Placa,
                CarroStatus = _carro.Status.ToString(),
                ModeloNome = _modelo.Nome,

            };
            _vendaDTOAdd = new VendaDTOAdd
            {
                ValorVenda = _venda.ValorVenda,
                DataVenda = _venda.DataVenda.ToString("dd/MM/yyyy"),
                CarroId = _carro.Id
            };
          _vendaDTOUpdate  = new VendaDTOUpdate
            {
                Id = 1,
                ValorVenda = 16000.00m,
                DataVenda = DateTime.Now.ToString("dd/MM/yyyy"),
                CarroId = 1
            };
        }

        [Fact]
        public async Task GetVendasAsync_DeveRetornarVendas()
        {
            _venda.SetRowVersion([1, 2, 3, 4]);
            _venda.GetType().GetProperty("Id")?.SetValue(_venda, 1);
            _unitOfWorkMock.Setup(v => v.Vendas.GetVendasAsync()).ReturnsAsync(new List<Venda> { _venda });
            _mapperMock.Setup(m => m.Map<IEnumerable<VendaDTO>>(It.IsAny<object>()))
                .Returns([_vendaDto]);
            var vendas = await _vendaService.GetVendasAsync();
            Assert.NotNull(vendas);
            Assert.IsAssignableFrom<IEnumerable<VendaDTO>>(vendas);
            Assert.Single(vendas);
            Assert.Equal(_vendaDto.Id, vendas.First().Id);

        }

        [Fact]
        public async Task GetVendaByIdAsync_DeveRetornarVenda()

        {
            _vendaDto.GetType().GetProperty("Id")?.SetValue(_vendaDto, 1);
            _venda.GetType().GetProperty("Id")?.SetValue(_venda, 1);
            _unitOfWorkMock.Setup(v => v.Vendas.GetVendaByIdAsync(1)).ReturnsAsync(_venda);
            _mapperMock.Setup(m => m.Map<VendaDTO>(It.IsAny<object>())).Returns(_vendaDto);

            var vendas = await _vendaService.GetVendaByIdAsync(1);
            Assert.NotNull(vendas);
            Assert.IsAssignableFrom<VendaDTO>(vendas);
            Assert.Equal(_vendaDto.Id, vendas.Id);
        }

        [Fact]
        public async Task GetVendaByIdAsyn_DeveRetornarNull()

        {
            _vendaDto.GetType().GetProperty("Id")?.SetValue(_vendaDto, 1);
            _venda.GetType().GetProperty("Id")?.SetValue(_venda, 1);
            _unitOfWorkMock.Setup(v => v.Vendas.GetVendaByIdAsync(1)).ReturnsAsync((Venda)null!);
            _mapperMock.Setup(m => m.Map<VendaDTO>(It.IsAny<object>())).Returns(_vendaDto);

            var vendasNull = await _vendaService.GetVendaByIdAsync(1);
            Assert.Null(vendasNull);

        }

        [Fact]
        public async Task GetVendaByIdAsyn_DeveRetornarDomainExcessao()
        {
            _unitOfWorkMock.Setup(v => v.Vendas.GetVendaByIdAsync(1)).ThrowsAsync(new DomainException("Erro ao buscar a venda."));
            var domainEx = await Assert.ThrowsAsync<DomainException>(() => _vendaService.GetVendaByIdAsync(1));
            Assert.Equal("Erro ao buscar a venda.", domainEx.Message);
        }

        [Fact]
        public async Task GetVendaByIdAsyn_DeveRetornarExcessao()
        {
            _unitOfWorkMock.Setup(v => v.Vendas.GetVendaByIdAsync(1)).ThrowsAsync(new Exception());
            var domainEx = await Assert.ThrowsAsync<Exception>(() => _vendaService.GetVendaByIdAsync(1));
            Assert.Equal("Aconteceu um erro inesperado.", domainEx.Message);
        }
        [Fact]
        public async Task DeleteAsync_DeveLancarDomainException()
        {
            _unitOfWorkMock.Setup(v => v.Vendas.GetVendaByIdAsync(1)).ReturnsAsync((Venda)null!);
            var domainEx = await Assert.ThrowsAsync<DomainException>(() => _vendaService.DeleteAsync(1));
            Assert.Equal("Venda não encontrada!", domainEx.Message);
        }
        [Fact]
        public async Task DeleteAsync_DeveLancarException()
        {
            _unitOfWorkMock.Setup(v => v.Vendas.GetVendaByIdAsync(1)).ReturnsAsync(_venda);
            _unitOfWorkMock.Setup(v => v.Vendas.DeleteAsync(It.IsAny<Venda>())).ThrowsAsync(new Exception("Erro ao excluir a venda."));
            var ex = await Assert.ThrowsAsync<Exception>(() => _vendaService.DeleteAsync(1));
            Assert.Equal("Aconteceu um erro inesperado ao excluir.", ex.Message);
        }
        [Fact]
        public async Task DeleteAsync_DeveLancarExceptionCarroNaoEncontrado()
        {
            _venda.GetType().GetProperty("Id")?.SetValue(_venda, 1);
            _carro.GetType().GetProperty("Id")?.SetValue(_carro, 1);
            _modelo.GetType().GetProperty("Id")?.SetValue(_modelo, 1);
            _unitOfWorkMock.Setup(v => v.Vendas.GetVendaByIdAsync(1)).ReturnsAsync(_venda);
            _unitOfWorkMock.Setup(v => v.Carros.GetCarroByIdAsync(1)).ReturnsAsync((Carro)null!);
            var ex = await Assert.ThrowsAsync<DomainException>(() => _vendaService.DeleteAsync(1));
            Assert.Equal("O carro não pode ser nulo.", ex.Message);
        }
        [Fact]
        public async Task DeleteAsync_DeveLancarExceptionModeloNaoEncontrado()
        {

            _venda.GetType().GetProperty("Id")?.SetValue(_venda, 1);
            _venda.GetType().GetProperty("CarroId")?.SetValue(_venda, 1);
            _carro.GetType().GetProperty("Id")?.SetValue(_carro, 1);
            _modelo.GetType().GetProperty("Id")?.SetValue(_modelo, 1);
            _unitOfWorkMock.Setup(v => v.Vendas.GetVendaByIdAsync(1)).ReturnsAsync(_venda);
            _unitOfWorkMock.Setup(v => v.Carros.GetCarroByIdAsync(1)).ReturnsAsync(_carro);
            _unitOfWorkMock.Setup(v => v.Modelos.GetModeloByIdAsync(1)).ReturnsAsync((Modelo)null!);
            var ex = await Assert.ThrowsAsync<DomainException>(() => _vendaService.DeleteAsync(1));
            Assert.Equal("O modelo não pode ser nulo.", ex.Message);
        }
        [Fact]
        public async Task DeleteAsync_DeveDeletarVendaComSucesso()
        {
            _venda.GetType().GetProperty("Id")?.SetValue(_venda, 1);
            _carro.GetType().GetProperty("Id")?.SetValue(_carro, 1);
            _venda.GetType().GetProperty("CarroId")?.SetValue(_venda, 1);
            _carro.GetType().GetProperty("ModeloId")?.SetValue(_carro, 1);
            _modelo.GetType().GetProperty("Id")?.SetValue(_modelo, 1);
            _unitOfWorkMock.Setup(v => v.Vendas.GetVendaByIdAsync(1)).ReturnsAsync(_venda);
            _unitOfWorkMock.Setup(v => v.Carros.GetCarroByIdAsync(1)).ReturnsAsync(_carro);
            _unitOfWorkMock.Setup(v => v.Modelos.GetModeloByIdAsync(1)).ReturnsAsync(_modelo);
            _unitOfWorkMock.Setup(v => v.Vendas.DeleteAsync(It.IsAny<Venda>())).ReturnsAsync(true);
            var result = await _vendaService.DeleteAsync(1);
            Assert.True(result);
            _unitOfWorkMock.Verify(v => v.Vendas.DeleteAsync(It.IsAny<Venda>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_DeveLancarDomainExceptionDataInvalida()
        {
            _vendaDTOAdd.DataVenda = "2024/02/2023";
            _mapperMock.Setup(v => v.Map<Venda>(_vendaDTOAdd)).Returns(_venda);
            _unitOfWorkMock.Setup(v => v.Carros.GetCarroByIdAsync(1)).ReturnsAsync(_carro);
            var domainEx = await Assert.ThrowsAsync<DomainException>(() => _vendaService.CreateAsync(_vendaDTOAdd));
            Assert.Equal("Data da venda inválida.", domainEx.Message);
        }

        [Fact]
        public async Task CreateAsync_DeveLancarDomainExceptionDataFutura()
        {
            _vendaDTOAdd.DataVenda = DateTime.Now.AddDays(1).AddMonths(2).ToString("dd/MM/yyyy");
            _mapperMock.Setup(v => v.Map<Venda>(_vendaDTOAdd)).Returns(_venda);
            var domainEx = await Assert.ThrowsAsync<DomainException>(() => _vendaService.CreateAsync(_vendaDTOAdd));
            Assert.Equal("A data da venda não pode ser no futuro.", domainEx.Message);
        }
        [Fact]
        public async Task CreateAsync_DeveLancarDomainCarroNaoDisponivel()
        {
            _carro.GetType().GetProperty("Status")?.SetValue(_carro, EnumCarroStatus.Alugado);
            _venda.GetType().GetProperty("CarroId")?.SetValue(_venda, 1);
            _vendaDTOAdd.CarroId = 1;
            _mapperMock.Setup(v => v.Map<Venda>(_vendaDTOAdd)).Returns(_venda);
            _unitOfWorkMock.Setup(v => v.Carros.GetCarroByIdAsync(1)).ReturnsAsync(_carro);
            _unitOfWorkMock.Setup(v => v.Vendas.CreateAsync(_venda)).ReturnsAsync(_venda);
            var domainEx = await Assert.ThrowsAsync<DomainException>(() => _vendaService.CreateAsync(_vendaDTOAdd));
            Assert.Contains("não está disponível para venda!", domainEx.Message);
        }
        [Fact]
        public async Task CreateAsync_DeveCriarVendaComSucesso()
        {
            _venda.GetType().GetProperty("CarroId")?.SetValue(_venda, 1);
            _vendaDTOAdd.CarroId = 1;
            _mapperMock.Setup(v => v.Map<Venda>(_vendaDTOAdd)).Returns(_venda);
            _mapperMock.Setup(v => v.Map<VendaDTO>(_venda)).Returns(_vendaDto);
            _unitOfWorkMock.Setup(v => v.Carros.GetCarroByIdAsync(1)).ReturnsAsync(_carro);
            _unitOfWorkMock.Setup(v => v.Vendas.CreateAsync(_venda)).ReturnsAsync(_venda);
            var result = await _vendaService.CreateAsync(_vendaDTOAdd);
            
            Assert.NotNull(result);
            Assert.IsAssignableFrom<VendaDTO>(result);
            Assert.Equal(_vendaDto.Id, result.Id);
        }

        [Fact]
        public async Task UpdateAsync_DeveLancarDomainExceptionVendaNaoEncontrada()
        {
            _unitOfWorkMock.Setup(v => v.Vendas.GetVendaByIdAsync(1)).ReturnsAsync((Venda)null!);
            var domainEx = await Assert.ThrowsAsync<DomainException>(() => _vendaService.UpdateAsync(_vendaDTOUpdate));
            Assert.Equal("Venda não encontrada!", domainEx.Message);
        }
        [Fact]
        public async Task UpdateAsync_DeveLancarDomainExceptionCarroNaoEncontrado()
        {
            _venda.GetType().GetProperty("Id")?.SetValue(_venda, 1);
            _venda.GetType().GetProperty("CarroId")?.SetValue(_venda, 1);
            _unitOfWorkMock.Setup(v => v.Vendas.GetVendaByIdAsync(1)).ReturnsAsync(_venda);
            _unitOfWorkMock.Setup(v => v.Carros.GetCarroByIdAsync(1)).ReturnsAsync((Carro)null!);
            var domainEx = await Assert.ThrowsAsync<DomainException>(() => _vendaService.UpdateAsync(_vendaDTOUpdate));
            Assert.Equal("O carro não pode ser nulo.", domainEx.Message);
        }

        [Fact]
        public async Task UpdateAsync_DeveLancarDomainExceptionDataInvalida()
        {
            _vendaDTOUpdate.DataVenda = "2024/02/2023";
            _venda.GetType().GetProperty("Id")?.SetValue(_venda, 1);
            _venda.GetType().GetProperty("CarroId")?.SetValue(_venda, 1);
            _unitOfWorkMock.Setup(v => v.Vendas.GetVendaByIdAsync(1)).ReturnsAsync(_venda);
            _unitOfWorkMock.Setup(v => v.Carros.GetCarroByIdAsync(1)).ReturnsAsync(_carro);
            var domainEx = await Assert.ThrowsAsync<DomainException>(() => _vendaService.UpdateAsync(_vendaDTOUpdate));
            Assert.Equal("Data da venda inválida.", domainEx.Message);
        }
        [Fact]
        public async Task UpdateAsync_DeveLancarDomainExceptionDataFutura()
        {
            _vendaDTOUpdate.DataVenda = DateTime.Now.AddDays(1).AddMonths(2).ToString("dd/MM/yyyy");
            _venda.GetType().GetProperty("Id")?.SetValue(_venda, 1);
            _venda.GetType().GetProperty("CarroId")?.SetValue(_venda, 1);
            _unitOfWorkMock.Setup(v => v.Vendas.GetVendaByIdAsync(1)).ReturnsAsync(_venda);
            _unitOfWorkMock.Setup(v => v.Carros.GetCarroByIdAsync(1)).ReturnsAsync(_carro);
            var domainEx = await Assert.ThrowsAsync<DomainException>(() => _vendaService.UpdateAsync(_vendaDTOUpdate));
            Assert.Equal("A data da venda não pode ser no futuro.", domainEx.Message);
        }
        [Fact]
        public async Task UpdateAsync_DeveLancarDomainExceptionCarroNaoDisponivel()
        {
            _venda.GetType().GetProperty("Id")?.SetValue(_venda, 1);
            _venda.GetType().GetProperty("CarroId")?.SetValue(_venda, 2);
            _carro.GetType().GetProperty("Status")?.SetValue(_carro, EnumCarroStatus.Alugado);
            _vendaDto.Id = 1;
            _unitOfWorkMock.Setup(v => v.Vendas.GetVendaByIdAsync(1)).ReturnsAsync(_venda);
            _unitOfWorkMock.Setup(v => v.Carros.GetCarroByIdAsync(1)).ReturnsAsync(_carro);
            var domainEx = await Assert.ThrowsAsync<DomainException>(() => _vendaService.UpdateAsync(_vendaDTOUpdate));
            Assert.Contains("O carro com a placa", domainEx.Message);
        }
        [Fact]
        public async Task UpdateAsync_DeveAtualizarVendaComMesmoCarro()
        {
            _venda.GetType().GetProperty("Id")?.SetValue(_venda, 1);
            _venda.GetType().GetProperty("CarroId")?.SetValue(_venda, 1);
            _vendaDto.Id = 1;
            _unitOfWorkMock.Setup(v => v.Vendas.GetVendaByIdAsync(1)).ReturnsAsync(_venda);
            _unitOfWorkMock.Setup(v => v.Carros.GetCarroByIdAsync(1)).ReturnsAsync(_carro);
            _unitOfWorkMock.Setup(v => v.Vendas.UpdateAsync(_venda)).ReturnsAsync(_venda);
            _mapperMock.Setup(m => m.Map<VendaDTO>(_venda)).Returns(_vendaDto);
            var vendaUpdateResult = await  _vendaService.UpdateAsync(_vendaDTOUpdate);
            _unitOfWorkMock.Verify(v => v.Vendas.UpdateAsync(It.IsAny<Venda>()), Times.Once);
            Assert.Equal(_vendaDto.Id, vendaUpdateResult.Id);
            Assert.Equal(_vendaDto.ValorVenda, vendaUpdateResult.ValorVenda);
           

        }

        [Fact]
        public async Task UpdateAsync_DeveAtualizarVendaComNovoCarro()
        {
            _venda.GetType().GetProperty("Id")?.SetValue(_venda, 1);
            _venda.GetType().GetProperty("CarroId")?.SetValue(_venda, 2);
            _vendaDto.Id = 1;
            _unitOfWorkMock.Setup(v => v.Vendas.GetVendaByIdAsync(1)).ReturnsAsync(_venda);
            _unitOfWorkMock.Setup(v => v.Carros.GetCarroByIdAsync(1)).ReturnsAsync(_carro);
            _unitOfWorkMock.Setup(v => v.Vendas.UpdateAsync(_venda)).ReturnsAsync(_venda);
            _mapperMock.Setup(m => m.Map<VendaDTO>(_venda)).Returns(_vendaDto);
            var vendaUpdateResult = await _vendaService.UpdateAsync(_vendaDTOUpdate);
            _unitOfWorkMock.Verify(v => v.Vendas.UpdateAsync(It.IsAny<Venda>()), Times.Once);
            Assert.Equal(_vendaDto.Id, vendaUpdateResult.Id);
            Assert.Equal(_vendaDto.ValorVenda, vendaUpdateResult.ValorVenda);
        }
       
        
    }
}
