using LocaCarros.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LocaCarros.API.Filters
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var (statusCode, message) = MapException(context.Exception);
          
           
            context.Result = new JsonResult(new { success = false, error = message })
            {
                StatusCode = statusCode
            };

            context.ExceptionHandled = true;
        }

        private static (int statusCode, string message) MapException(Exception ex)
        {
            return ex switch
            {
                DomainException domainEx => (StatusCodes.Status400BadRequest, domainEx.Message),
                KeyNotFoundException keyNotFoundEx => (StatusCodes.Status404NotFound, keyNotFoundEx.Message),
                _ => (StatusCodes.Status500InternalServerError, "Erro inesperado. Tente novamente mais tarde.")
            };
        }
    }
}
