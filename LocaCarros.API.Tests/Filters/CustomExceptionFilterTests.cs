using LocaCarros.API.Filters;
using LocaCarros.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LocaCarros.API.Tests.Filters
{
    public class CustomExceptionFilterTests
    {
        [Fact]
        public void Deve_retornar_BadRequest_quando_DomainException()
        {
            var exception = new DomainException("Erro de domínio");
            var context = CriarExceptionContext(exception);
            var filtro = new CustomExceptionFilter();

            filtro.OnException(context);

            var resultado = Assert.IsType<JsonResult>(context.Result);
            var messageErro = resultado.Value?.GetType().GetProperty("error")?.GetValue(resultado.Value);
            Assert.Equal(400, resultado.StatusCode);
            Assert.NotNull(messageErro);
            Assert.Equal("Erro de domínio", messageErro);
            Assert.True(context.ExceptionHandled);
        }

        [Fact]
        public void Deve_retornar_NotFound_quando_KeyNotFoundException()
        {
            var exception = new KeyNotFoundException("Item não encontrado");
            var context = CriarExceptionContext(exception);
            var filtro = new CustomExceptionFilter();

            filtro.OnException(context);

            var resultado = Assert.IsType<JsonResult>(context.Result);
            var messageErro = resultado.Value?.GetType().GetProperty("error")?.GetValue(resultado.Value);
            Assert.Equal(404, resultado.StatusCode);
            Assert.NotNull(messageErro);
            Assert.Equal("Item não encontrado", messageErro);
            Assert.True(context.ExceptionHandled);
        }

        [Fact]
        public void Deve_retornar_InternalServerError_quando_excecao_generica()
        {
            var exception = new Exception("Erro desconhecido");
            var context = CriarExceptionContext(exception);
            var filtro = new CustomExceptionFilter();

            filtro.OnException(context);

            var resultado = Assert.IsType<JsonResult>(context.Result);
            var messageErro = resultado.Value?.GetType().GetProperty("error")?.GetValue(resultado.Value);
            Assert.Equal(500, resultado.StatusCode);
            Assert.NotNull(messageErro);
            Assert.Equal("Erro inesperado. Tente novamente mais tarde.", messageErro);
            Assert.True(context.ExceptionHandled);
        }

        private ExceptionContext CriarExceptionContext(Exception ex)
        {
            var actionContext = new ActionContext
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
                ActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor()
            };

            return new ExceptionContext(actionContext, new List<IFilterMetadata>())
            {
                Exception = ex
            };
        }
    }
}
