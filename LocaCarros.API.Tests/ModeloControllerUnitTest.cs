using LocaCarros.API.Controllers;
using LocaCarros.Application.DTOs.ModelosDtos;
using LocaCarros.Application.Interfaces;
using LocaCarros.Domain.Enuns;
using Microsoft.AspNetCore.Mvc;
using Moq;
namespace LocaCarros.API.Tests
{
    public class ModeloControllerUnitTest
    {
        private readonly Mock<IModeloService> _modeloServiceMock;
        private readonly ModeloController _controller;

        public ModeloControllerUnitTest()
        {
            _modeloServiceMock = new Mock<IModeloService>();
            _controller = new ModeloController(_modeloServiceMock.Object);
        }

        private static ModeloDTO GetMockModeloDTO() => new()
        {
            Id = 1,
            MarcaId = 1,
            MarcaNome = "teste",
            Motorizacao = 1,
            Nome = "Modelo",
            TipoCarroceria = EnumTipoCarroceria.Hatch,
            Versao = "1123"
        };

        [Fact]
        public async Task Get_DeveRetornarListaDeModelos()
        {
            var modelos = new List<ModeloDTO> { GetMockModeloDTO() };
            _modeloServiceMock.Setup(s => s.GetModelosAsync()).ReturnsAsync(modelos);

            var result = await _controller.Get();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var messageSucesso = okResult.Value?.GetType().GetProperty("data")?.GetValue(okResult.Value);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<ModeloDTO>>(messageSucesso);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task GetById_DeveRetornarModelo_QuandoEncontrado()
        {
            var modelo = GetMockModeloDTO();
            _modeloServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(modelo);

            var result = await _controller.Get(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var messageSucesso = okResult.Value?.GetType().GetProperty("data")?.GetValue(okResult.Value);
            var returnValue = Assert.IsType<ModeloDTO>(messageSucesso);
            Assert.Equal(1, returnValue.Id);
        }

        [Fact]
        public async Task GetById_DeveRetornarNotFound_QuandoNaoEncontrado()
        {
            _modeloServiceMock.Setup(s => s.GetByIdAsync(2)).ReturnsAsync((ModeloDTO?)null);

            var result = await _controller.Get(2);

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Nenhum modelo encontrado!", notFound?.Value?.GetType()?.GetProperty("message")?.GetValue(notFound.Value));
        }

        [Fact]
        public async Task Post_DeveCriarModeloComSucesso()
        {
            var modeloAdd = new ModeloDTOAdd
            {
                MarcaId = 1,
                Motorizacao = 1,
                Nome = "Modelo",
                TipoCarroceria = EnumTipoCarroceria.Hatch,
                Versao = "1123"
            };
            var modeloDTO = GetMockModeloDTO();

            _modeloServiceMock.Setup(s => s.AddAsync(modeloAdd)).ReturnsAsync(modeloDTO);

            var result = await _controller.Post(modeloAdd);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<ModeloDTO>(okResult.Value);
            Assert.Equal(modeloAdd.MarcaId, returnValue.MarcaId);
        }

        [Fact]
        public async Task Post_DeveRetornarBadRequest_QuandoCriacaoFalhar()
        {
            // Arrange
            var modeloAdd = new ModeloDTOAdd
            {
                MarcaId = 1,
                Motorizacao = 1,
                Nome = "Modelo",
                TipoCarroceria = EnumTipoCarroceria.Hatch,
                Versao = "1123"
            };

            _modeloServiceMock.Setup(s => s.AddAsync(modeloAdd)).ReturnsAsync((ModeloDTO)null!);

            var result = await _controller.Post(modeloAdd);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Falha ao criar modelo!", badRequest?.Value?.GetType()?.GetProperty("message")?.GetValue(badRequest.Value));
        }

        [Fact]
        public async Task Put_DeveRetornarBadRequest_QuandoIdNaoConferir()
        {
            var modeloUpdate = new ModeloDTOUpdate
            {
                Id = 1,
                MarcaId = 1,
                Motorizacao = 1,
                Nome = "Modelo",
                TipoCarroceria = EnumTipoCarroceria.Hatch,
                Versao = "1123"
            };

            var result = await _controller.Put(2, modeloUpdate);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Id do modelo não confere!", badRequest?.Value?.GetType()?.GetProperty("message")?.GetValue(badRequest.Value));
        }

        [Fact]
        public async Task Put_DeveAtualizarModeloComSucesso()
        {
            var modeloUpdate = new ModeloDTOUpdate
            {
                Id = 1,
                MarcaId = 1,
                Motorizacao = 1,
                Nome = "Modelo",
                TipoCarroceria = EnumTipoCarroceria.Hatch,
                Versao = "1123"
            };

            var modeloDTO = GetMockModeloDTO();
            _modeloServiceMock.Setup(s => s.UpdateAsync(modeloUpdate)).ReturnsAsync(modeloDTO);

            var result = await _controller.Put(1, modeloUpdate);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var messageSucesso = okResult.Value?.GetType().GetProperty("data")?.GetValue(okResult.Value);
            var returnValue = Assert.IsType<ModeloDTO>(messageSucesso);
            Assert.Equal(modeloUpdate.Id, returnValue.Id);
        }

        [Fact]
        public async Task Delete_DeveRetornarNotFound_QuandoModeloNaoExistir()
        {
            _modeloServiceMock.Setup(s => s.RemoveAsync(1)).ReturnsAsync(false);

            var result = await _controller.Delete(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Modelo não encontrado!", notFound?.Value?.GetType()?.GetProperty("message")?.GetValue(notFound.Value));
        }

        [Fact]
        public async Task Delete_DeveRemoverModeloComSucesso()
        {
            _modeloServiceMock.Setup(s => s.RemoveAsync(1)).ReturnsAsync(true);

            var result = await _controller.Delete(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal("Modelo removido com sucesso!", okResult?.Value?.GetType()?.GetProperty("message")?.GetValue(okResult.Value));
        }
    }
}