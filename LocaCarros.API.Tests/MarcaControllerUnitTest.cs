

using LocaCarros.API.Controllers;
using LocaCarros.Application.DTOs.MarcasDtos;
using LocaCarros.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LocaCarros.API.Tests
{
    public class MarcaControllerUnitTest
    {
        private readonly Mock<IMarcaService> _marcaServiceMock;
        private readonly MarcaController _controller;
        public MarcaControllerUnitTest()
        {
            _marcaServiceMock = new Mock<IMarcaService>();
            _controller = new MarcaController(_marcaServiceMock.Object);
        }

        private static MarcaDTO GetMockMarcaDTO(int id = 1, string nome = "Marca X") => new()
        {
            Id = id,
            Nome = nome
        };

        [Fact]
        public async Task Get_DeveRetornarListaDeMarcas()
        {
            var marcas = new List<MarcaDTO>
            {
                GetMockMarcaDTO(1, "Marca A"),
                GetMockMarcaDTO(2, "Marca B")
            };
            _marcaServiceMock.Setup(s => s.GetMarcasAsync()).ReturnsAsync(marcas);

            var result = await _controller.Get();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var messageSucesso = okResult.Value?.GetType().GetProperty("data")?.GetValue(okResult.Value);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<MarcaDTO>>(messageSucesso);
            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async Task GetById_DeveRetornarMarca_QuandoExistir()
        {
            var marca = GetMockMarcaDTO();
            _marcaServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(marca);

            var result = await _controller.Get(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var messageSucesso = okResult.Value?.GetType().GetProperty("data")?.GetValue(okResult.Value);
            var returnValue = Assert.IsType<MarcaDTO>(messageSucesso);
            Assert.Equal(1, returnValue.Id);
        }

        [Fact]
        public async Task GetById_DeveRetornarNotFound_QuandoNaoEncontrarMarca()
        {
            _marcaServiceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((MarcaDTO?)null);

            var result = await _controller.Get(99);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Contains("Nenhuma marca encontrada", notFoundResult.Value?.ToString());
        }

        [Fact]
        public async Task Post_DeveCriarMarcaComSucesso()
        {
            var addDto = new MarcaDTOAdd { Nome = "Nova Marca" };
            var createdDto = GetMockMarcaDTO(5, "Nova Marca");

            _marcaServiceMock.Setup(s => s.AddAsync(addDto)).ReturnsAsync(createdDto);

            var result = await _controller.Post(addDto);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var messageSucesso = okResult.Value?.GetType().GetProperty("data")?.GetValue(okResult.Value);
            var returnValue = Assert.IsType<MarcaDTO>(messageSucesso);
            Assert.Equal("Nova Marca", returnValue.Nome);
        }

        [Fact]
        public async Task Update_DeveRetornarBadRequest_QuandoIdDiferente()
        {
            var dto = GetMockMarcaDTO(10, "Marca Erro");

            var result = await _controller.Update(5, dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("Id da marca não confere", badRequest.Value?.ToString());
        }

        [Fact]
        public async Task Update_DeveRetornarMarcaAtualizada()
        {
            var dto = GetMockMarcaDTO(2, "Marca Atualizada");
            _marcaServiceMock.Setup(s => s.UpdateAsync(dto)).ReturnsAsync(dto);

            var result = await _controller.Update(2, dto);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var messageSucesso = okResult.Value?.GetType().GetProperty("data")?.GetValue(okResult.Value);
            var returnValue = Assert.IsType<MarcaDTO>(messageSucesso);
            Assert.Equal("Marca Atualizada", returnValue.Nome);
        }

        [Fact]
        public async Task Delete_DeveRetornarOk_QuandoRemovido()
        {
            _marcaServiceMock.Setup(s => s.RemoveAsync(1)).ReturnsAsync(true);

            var result = await _controller.Delete(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Contains("Marca removida com sucesso", okResult.Value?.ToString());
        }

        [Fact]
        public async Task Delete_DeveRetornarNotFound_QuandoMarcaNaoExiste()
        {
            _marcaServiceMock.Setup(s => s.RemoveAsync(99)).ReturnsAsync(false);

            var result = await _controller.Delete(99);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Contains("Marca não encontrada", notFoundResult.Value?.ToString());
        }

        [Fact]
        public async Task Delete_DeveRetornarBadRequest_QuandoOcorrerExcecao()
        {
            _marcaServiceMock.Setup(s => s.RemoveAsync(5))
                .ThrowsAsync(new InvalidOperationException("Erro de negócio"));

            var result = await _controller.Delete(5);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("Erro de negócio", badRequest.Value?.ToString());
        }

    }
}
