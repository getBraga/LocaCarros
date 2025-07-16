using LocaCarros.API.Controllers;
using LocaCarros.Application.DTOs.AlugueisDtos;
using LocaCarros.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LocaCarros.API.Tests
{
    public class AluguelControllerTest
    {
        private readonly Mock<IAluguelService> _aluguelServiceMock;
        private readonly AluguelController _aluguelController;
        public AluguelControllerTest()
        {
            _aluguelServiceMock = new Mock<IAluguelService>();
            _aluguelController = new AluguelController(_aluguelServiceMock.Object);
        }
        private static readonly DateTime dateNow = DateTime.Now;

        private static AluguelDTO GetMockAluguelDTO() => new()
        {
            Id = 1,
            CarroAno = dateNow.Year,
            CarroCor = "Preto",
            CarroDataFabricacao = dateNow.AddYears(-2).ToString(),
            CarroPlaca = "ABC-1234",
            DataInicio = dateNow.ToString(),
            DataDevolucao = dateNow.AddYears(1).ToString(),
            CarroId = 1,
            CarroStatus = "Disponível",
            ModeloNome = "Fusca",
            ValorAluguel = 30000.00m
        };

        [Fact]
        public async Task Get_DeveRetornarListaDeAlugueis()
        {
            var alugueis = new List<AluguelDTO> { GetMockAluguelDTO() };
            _aluguelServiceMock.Setup(a => a.GetAlugueisAsync()).ReturnsAsync(alugueis);
            var alugueisResult = await _aluguelController.Get();
            var okResult = Assert.IsType<OkObjectResult>(alugueisResult.Result);
            var dataSucesso = okResult.Value?.GetType().GetProperty("data")?.GetValue(okResult.Value);
            var returnValue = Assert.IsType<IEnumerable<AluguelDTO>>(dataSucesso, exactMatch: false);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task Get_DeveRetornarAluguelPorId()
        {
            var aluguel = GetMockAluguelDTO();
            _aluguelServiceMock.Setup(a => a.GetAluguelByIdAsync(1)).ReturnsAsync(aluguel);
            var aluguelResult = await _aluguelController.Get(1);
            var okResult = Assert.IsType<OkObjectResult>(aluguelResult.Result);
            var dataSucesso = okResult.Value?.GetType()?.GetProperty("data")?.GetValue(okResult.Value);
            var returnValue = Assert.IsType<AluguelDTO>(dataSucesso, exactMatch: false);
            Assert.Equal(1, returnValue.Id);

        }

        [Fact]
        public async Task Get_DeveRetornarNotFound_QuandoAluguelNaoEncontrado()
        {
            _aluguelServiceMock.Setup(a => a.GetAluguelByIdAsync(1)).ReturnsAsync((AluguelDTO?)null);
            var aluguelResult = await _aluguelController.Get(1);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(aluguelResult.Result);
            var mensagemErro = notFoundResult.Value?.GetType()?.GetProperty("message")?.GetValue(notFoundResult.Value);
            Assert.Equal("Aluguel não encontrado.", mensagemErro);
        }

        [Fact]
        public async Task Delete_DeveRetornarOk_QuandoAluguelExcluidoComSucesso()
        {
            _aluguelServiceMock.Setup(a => a.DeleteAsync(1)).ReturnsAsync(true);
            var result = await _aluguelController.Delete(1);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var mensagemSucesso = okResult.Value?.GetType()?.GetProperty("message")?.GetValue(okResult.Value);
            Assert.Equal("Aluguel removido com sucesso!", mensagemSucesso);
        }

        [Fact]
        public async Task Delete_DeveRetornarNotFound_QuandoAluguelNaoEncontrado()
        {
            _aluguelServiceMock.Setup(a => a.DeleteAsync(1)).ReturnsAsync(false);
            var result = await _aluguelController.Delete(1);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var mensagemErro = notFoundResult.Value?.GetType()?.GetProperty("message")?.GetValue(notFoundResult.Value);
            Assert.Equal("Aluguel não encontrado ou não pode ser excluído.", mensagemErro);
        }


        [Fact]
        public async Task Put_DeveRetornarOk_QuandoAluguelAtualizadoComSucesso()
        {
            var aluguelDTOUpdate = new AluguelDTOUpdate
            {
                Id = 1,
                DataInicio = dateNow.ToString(),
                DataDevolucao = dateNow.AddYears(1).ToString(),
                CarroId = 1,
                ValorAluguel = 30000.00m
            };
            _aluguelServiceMock.Setup(a => a.UpdateAsync(aluguelDTOUpdate)).ReturnsAsync(GetMockAluguelDTO());
            var result = await _aluguelController.Put(1, aluguelDTOUpdate);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dataSucesso = okResult.Value?.GetType()?.GetProperty("data")?.GetValue(okResult.Value);
            var returnValue = Assert.IsType<AluguelDTO>(dataSucesso, exactMatch: false);
            Assert.Equal(1, returnValue.Id);
        }

        [Fact]
        public async Task Put_DeveRetornarBadRequest_QuandoIdNaoForIgual()
        {
            var aluguelDTOUpdate = new AluguelDTOUpdate
            {
                Id = 1,
                DataInicio = dateNow.ToString(),
                DataDevolucao = dateNow.AddYears(1).ToString(),
                CarroId = 1,
                ValorAluguel = 30000.00m
            };

            var aluguelReturn = await _aluguelController.Put(2, aluguelDTOUpdate);
            var result = Assert.IsType<BadRequestObjectResult>(aluguelReturn.Result);
            var badRequestResturn = result.Value?.GetType().GetProperty("message")?.GetValue(result.Value);
            Assert.Equal("ID do aluguel não corresponde ao ID fornecido na URL.", badRequestResturn);
        }
        [Fact]
        public async Task Put_DeveNotFound_QuandoRetornarNull()
        {
            var aluguelDTOUpdate = new AluguelDTOUpdate
            {
                Id = 1,
                DataInicio = "2023-01-01",
                DataDevolucao = "2023-10-01",
                CarroId = 1,
                ValorAluguel = 30000.00m
            };
            _aluguelServiceMock.Setup(a => a.UpdateAsync(aluguelDTOUpdate)).ReturnsAsync((AluguelDTO?)null!);
            var aluguelReturn = await _aluguelController.Put(1, aluguelDTOUpdate);
            var result = Assert.IsType<NotFoundObjectResult>(aluguelReturn.Result);
            var notFoundResturn = result.Value?.GetType().GetProperty("message")?.GetValue(result.Value);
            Assert.Equal("Aluguel não encontrado.", notFoundResturn);
        }

        [Fact]
        public async Task Post_DeveRetornarOk_QuandoForCriarAluguel()
        {
            var aluguelCreate = new AluguelDTOAdd
            {
                CarroId = 1,
                DataInicio = dateNow.ToString(),
                DataDevolucao = dateNow.AddYears(1).ToString(),
                ValorAluguel = 1234.45m
            };

            _aluguelServiceMock.Setup(a => a.CreateAsync(aluguelCreate)).ReturnsAsync(GetMockAluguelDTO());
            var aluguel = await _aluguelController.Post(aluguelCreate);
            var result = Assert.IsType<OkObjectResult>(aluguel.Result);
            var dataResult = result.Value?.GetType().GetProperty("data")?.GetValue(result.Value);
            var returnValue = Assert.IsType<AluguelDTO>(dataResult, exactMatch: false);
            Assert.Equal(1, returnValue.Id);

        }
    }
}
