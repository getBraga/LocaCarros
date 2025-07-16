using LocaCarros.API.Controllers;
using LocaCarros.Application.DTOs.CarrosDtos;
using LocaCarros.Application.Interfaces;
using LocaCarros.Domain.Enuns;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.API.Tests
{
    public class CarroControllerTest
    {
        private readonly Mock<ICarroService> _carroServiceMock;
        private readonly CarroController _carroController;

        public CarroControllerTest()
        {
            _carroServiceMock = new Mock<ICarroService>();
            _carroController = new CarroController(_carroServiceMock.Object);
        }
        private static readonly DateTime dateNow = DateTime.Now;
        private static CarroDTO GetMockCarroDTO() => new() {
            Id = 1,
            Placa = "ABC-1234",
            Cor = "Preto",
            Ano = dateNow.Year,
            DataFabricacao = dateNow.ToString(),
            ModeloNome = "Fusca",
            Status = "Disponível",
            ModeloId = 1,
            ModeloMotorizacao = 1.0m,

        };
         [Fact]
        public async Task Get_DeveRetornarListaDeCarros()
        {
            var carros = new List<CarroDTO> { GetMockCarroDTO() };
            _carroServiceMock.Setup(s => s.GetCarrosAsync()).ReturnsAsync(carros);
            var result = await _carroController.Get();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dataResult = okResult.Value?.GetType().GetProperty("data")?.GetValue(okResult.Value);
            var returnValue = Assert.IsType<IEnumerable<CarroDTO>>(dataResult, exactMatch: false);
            Assert.Single(returnValue);
        }
        [Fact]
        public async Task GetById_DeveRetornarCarro()
        {
            _carroServiceMock.Setup(c => c.GetCarroByIdAsync(1)).ReturnsAsync(GetMockCarroDTO());
            var carro = await _carroController.Get(1);
            var okResult = Assert.IsType<OkObjectResult>(carro.Result);
            var dataResult = okResult.Value?.GetType().GetProperty("data")?.GetValue(okResult.Value);
            var returnValue = Assert.IsType<CarroDTO>(dataResult, exactMatch: false);
            Assert.Equal(1, returnValue.Id);
        }
        [Fact]
        public async Task GetById_DeveRetornarNotFound_QuandoBuscarCarro()
        {
            _carroServiceMock.Setup(c => c.GetCarroByIdAsync(1)).ReturnsAsync((CarroDTO?)null);
            var carro = await _carroController.Get(1);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(carro.Result);
            var messageNotFound = notFoundResult.Value?.GetType().GetProperty("message")?.GetValue(notFoundResult.Value);
          
            Assert.Equal("O carro não foi encontrado!", messageNotFound);

        }
        [Fact]
        public async Task Post_DeveRetornarOk()
        {
          
            var carroDtoAdd = new CarroDTOAdd
            {
                Ano = dateNow.Year,
                Cor = "Vermelho",
                DataFabricacao = dateNow,
                ModeloId = 1,
                Placa = "ALJ2255",
                Status = EnumCarroStatus.Disponivel
            };

            _carroServiceMock.Setup(c => c.CreateAsync(carroDtoAdd)).ReturnsAsync(GetMockCarroDTO);
            var carro = await _carroController.Post(carroDtoAdd);
            var okResult = Assert.IsType<OkObjectResult>(carro.Result);
            var dataResult = okResult.Value?.GetType()?.GetProperty("data")?.GetValue(okResult.Value);
            var carroDtoResult = Assert.IsType<CarroDTO>(dataResult);
            Assert.Equal(1, carroDtoResult.Id);
        }

        [Fact]
        public async Task Put_DeveRetornarOk_QuandoAtualizarCarro()
        {
            var carroDtoUpdate = new CarroDTOUpdate
            {
                Id = 1,
                Ano = dateNow.Year,
                Cor = "Azul",
                DataFabricacao = dateNow,
                ModeloId = 1,
                Placa = "XYZ-9876",
                Status = EnumCarroStatus.Disponivel
            };
            _carroServiceMock.Setup(c => c.UpdateAsync(carroDtoUpdate)).ReturnsAsync(GetMockCarroDTO);
            var carro = await _carroController.Put(1, carroDtoUpdate);
            var okResult = Assert.IsType<OkObjectResult>(carro.Result);
            var dataResult = okResult.Value?.GetType()?.GetProperty("data")?.GetValue(okResult.Value);
            var carroDtoResult = Assert.IsType<CarroDTO>(dataResult);
            Assert.Equal(1, carroDtoResult.Id);
        }
        [Fact]
        public async Task Put_DeveRetornarNotFound_QuandoAtualizarCarroInexistente()
        {
            var carroDtoUpdate = new CarroDTOUpdate
            {
                Id = 1,
                Ano = dateNow.Year,
                Cor = "Azul",
                DataFabricacao = dateNow,
                ModeloId = 1,
                Placa = "XYZ-9876",
                Status = EnumCarroStatus.Disponivel
            };
            _carroServiceMock.Setup(c => c.UpdateAsync(carroDtoUpdate)).ReturnsAsync((CarroDTO?)null!);
            var carro = await _carroController.Put(1, carroDtoUpdate);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(carro.Result);
            var messageNotFound = notFoundResult.Value?.GetType()?.GetProperty("message")?.GetValue(notFoundResult.Value);
            Assert.Equal("Carro não encontrado!", messageNotFound);
        }
        [Fact]
        public async Task Put_DeveRetornarBadRequest_QuantoAtualizarCarroIdNaoConfere()
        {
            var carroDtoUpdate = new CarroDTOUpdate
            {
                Id = 1,
                Ano = dateNow.Year,
                Cor = "Azul",
                DataFabricacao = dateNow,
                ModeloId = 1,
                Placa = "XYZ-9876",
                Status = EnumCarroStatus.Disponivel
            };
            _carroServiceMock.Setup(c => c.UpdateAsync(carroDtoUpdate)).ReturnsAsync((CarroDTO?)null!);
            var carro = await _carroController.Put(2, carroDtoUpdate);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(carro.Result);
            var messageBadRequest = badRequestResult.Value?.GetType()?.GetProperty("message")?.GetValue(badRequestResult.Value);
            Assert.Equal("Id do carro não confere!", messageBadRequest);
        }
        [Fact]
        public async Task Delete_DeveRetornarOk_QuandoDeletar()
        {
            _carroServiceMock.Setup(v => v.DeleteAsync(1)).ReturnsAsync(true);
            var result = await _carroController.Delete(1);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var carroResult = okResult.Value?.GetType().GetProperty("message")?.GetValue(okResult.Value);
            Assert.Equal("Carro removido com sucesso!", carroResult);
        }
        [Fact]
        public async Task Delete_DeveRetornarNotFound_QuandoDeletar()
        {
            _carroServiceMock.Setup(c => c.DeleteAsync(1)).ReturnsAsync(false);
            var result = await _carroController.Delete(1);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var messageNotfound = notFoundResult.Value?
                                                .GetType()
                                                .GetProperty("message")?.GetValue(notFoundResult.Value);
            Assert.Equal("Carro não encontrado!", messageNotfound);
        }
    }
}
