using LocaCarros.API.Filters;

using LocaCarros.Infra.IOC;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;

using System.Text;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;  // Adicione o namespace para a tag ExcludeFromCodeCoverage

namespace LocaCarros.API
{
    [ExcludeFromCodeCoverage] 
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

     
            builder.Services.AddControllers();
            builder.Services.AddInfrastructureAPI(builder.Configuration);

            builder.Services.AddInfrastructureSwagger();

            builder.Services.AddOpenApi();
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<CustomExceptionFilter>();
            });
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "LocaCarros.API v1");
                    c.RoutePrefix = string.Empty;
                });
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        var errorMessage = contextFeature.Error.Message;
                        var result = JsonSerializer.Serialize(new { error = errorMessage });
                        await context.Response.WriteAsync(result);
                    }
                });
            });
            app.UseStatusCodePages();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}