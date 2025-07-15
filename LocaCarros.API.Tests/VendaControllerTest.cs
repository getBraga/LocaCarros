using LocaCarros.API.Controllers;
using LocaCarros.Application.DTOs.VendasDtos;
using LocaCarros.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.API.Tests
{
    public class VendaControllerTest
    {
        private readonly Mock<IVendaService> _vendaServiceMock;
        private readonly VendaController _vendaController;
        public VendaControllerTest()
        {
            _vendaServiceMock = new Mock<IVendaService>();
            _vendaController = new VendaController(_vendaServiceMock.Object);

        }

        private static VendaDTO GetMockVendaDTO() => new()
        {
            Id = 1,
            CarroAno = 2020,
            CarroCor = "Preto",
            CarroDataFabricacao = "2020-01-01",
            CarroPlaca = "ABC-1234",
            DataVenda = "2023-10-01",
            CarroStatus = "Disponível",
            ModeloNome = "Fusca",
            ValorVenda = 30000.00m
        };

        [Fact]
        public async Task Get_DeveRetornarListaDeVendas()
        {
            var vendas = new List<VendaDTO> { GetMockVendaDTO() };
            _vendaServiceMock.Setup(s => s.GetVendasAsync()).ReturnsAsync(vendas);
            var result = await _vendaController.Get();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<IEnumerable<VendaDTO>>(okResult.Value, exactMatch: false);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task GetById_DeveRetornarVenda_QuandoEncontrada()
        {
            var venda = GetMockVendaDTO();
            _vendaServiceMock.Setup(v => v.GetVendaByIdAsync(1)).ReturnsAsync(venda);
            var result = await _vendaController.Get(1);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var messageSucesso = okResult.Value?.GetType().GetProperty("data")?.GetValue(okResult.Value);
            var returnValue = Assert.IsType<VendaDTO>(messageSucesso);
            Assert.Equal(1, returnValue.Id);
        }

        [Fact]
        public async Task GetById_DeveRetornarNotFound_QuandoVendaNaoEncontrada()
        {

            _vendaServiceMock.Setup(v => v.GetVendaByIdAsync(1)).ReturnsAsync((VendaDTO?)null);
            var result = await _vendaController.Get(1);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var notFound = notFoundResult.Value?.GetType().GetProperty("message")?.GetValue(notFoundResult.Value);
            Assert.Equal("Venda não encontrada.", notFound);
        }

        [Fact]
        public async Task Delete_DeveRetornarOk_QuandoVendaExcluidaComSucesso()
        {
            _vendaServiceMock.Setup(v => v.DeleteAsync(1)).ReturnsAsync(true);
            var result = await _vendaController.Delete(1);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var vendaResult = okResult.Value?.GetType().GetProperty("message")?.GetValue(okResult.Value);
            Assert.Equal("Venda removida com sucesso!", vendaResult);
        }
        [Fact]
        public async Task Delete_DeveRetornarNotFound_QuandoVendaNaoEncontradaOuNaoPodeSerExcluida()
        {
            _vendaServiceMock.Setup(v => v.DeleteAsync(1)).ReturnsAsync(false);
            var result = await _vendaController.Delete(1);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var notFound = notFoundResult.Value?.GetType().GetProperty("message")?.GetValue(notFoundResult.Value);
            Assert.Equal("Venda não encontrada ou não pode ser excluída.", notFound);
        }

        [Fact]
        public async Task Put_DeveRetornarOk_QuandoVendaAtualizadaComSucesso()
        {
            var vendaUpdate = new VendaDTOUpdate
            {
                Id = 1,
                CarroId = 1,
                DataVenda = "2023-10-01",
                ValorVenda = 30000.00m
            };
            _vendaServiceMock.Setup(v => v.UpdateAsync(vendaUpdate)).ReturnsAsync(GetMockVendaDTO());
            var result = await _vendaController.Put(1, vendaUpdate);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var messageOk = okResult.Value?.GetType().GetProperty("data")?.GetValue(okResult.Value);
            var returnValue = Assert.IsType<VendaDTO>(messageOk);
            Assert.Equal(1, returnValue.Id);
        }

        [Fact]
        public async Task Put_DeveRetornarBadRequest_QuandoIdNaoConferir()
        {
            VendaDTOUpdate vendaUpdate = new VendaDTOUpdate(); // Simulando uma venda inválida
            var result = await _vendaController.Put(2, vendaUpdate);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var message = badRequestResult.Value?.GetType().GetProperty("message")?.GetValue(badRequestResult.Value);
            Assert.Equal("ID da venda não corresponde ao ID fornecido na URL.", message);
        }
        [Fact]
        public async Task Put_DeveRetornarNotFound_QuandoVendaNaoEncontrada()
        {
            var vendaUpdate = new VendaDTOUpdate
            {
                Id = 1,
                CarroId = 1,
                DataVenda = "2023-10-01",
                ValorVenda = 30000.00m
            };
            _vendaServiceMock.Setup(v => v.UpdateAsync(vendaUpdate)).ReturnsAsync((VendaDTO?)null!);
            var result = await _vendaController.Put(1, vendaUpdate);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var notFound = notFoundResult.Value?.GetType().GetProperty("message")?.GetValue(notFoundResult.Value);
            Assert.Equal("Venda não encontrada.", notFound);
        }
        [Fact]
        public async Task Post_DeveRetornarOk_QuandoVendaCriadaComSucesso()
        {
            var vendaAdd = new VendaDTOAdd
            {
                CarroId = 1,
                DataVenda = "2023-10-01",
                ValorVenda = 30000.00m
            };
            _vendaServiceMock.Setup(v => v.CreateAsync(vendaAdd)).ReturnsAsync(GetMockVendaDTO());
            var result = await _vendaController.Post(vendaAdd);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var messageOk = okResult.Value?.GetType().GetProperty("data")?.GetValue(okResult.Value);
            var returnValue = Assert.IsType<VendaDTO>(messageOk);
            Assert.Equal(1, returnValue.Id);
        }

        [Fact]
        public async Task Post_DeveRetornarBadRequest_QuandoVendaNula()
        {
            VendaDTOAdd? vendaAdd = null;
            var result = await _vendaController.Post(vendaAdd!);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var message = badRequestResult.Value?.GetType().GetProperty("message")?.GetValue(badRequestResult.Value);
            Assert.Equal("Venda não pode ser nula.", message);
        }
        [Fact]
        public async Task Post_DeveRetornarBadRequest_QuandoVendaInvalida()
        {
            VendaDTOAdd vendaAdd = new VendaDTOAdd(); // Simulando uma venda inválida
            var result = await _vendaController.Post(vendaAdd);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var message = badRequestResult.Value?.GetType().GetProperty("message")?.GetValue(badRequestResult.Value);
            Assert.Equal("Falha ao criar venda.", message);
        }
       
    }
}
