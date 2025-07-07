
using LocaCarros.API.Filters;
using LocaCarros.Application.Mappings;
using LocaCarros.Infra.IOC;
using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddInfrastructureAPI(builder.Configuration);
//builder.Services.AddEndpointsApiExplorer(); // Habilita a geração de endpoints para Swagger
builder.Services.AddInfrastructureSwagger(); 
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<CustomExceptionFilter>();
});
var app = builder.Build();


app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "LocaCarros.API v1");
    c.RoutePrefix = string.Empty;
});
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
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
        context.Response.StatusCode = StatusCodes.Status400BadRequest; // ou outro código
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

app.UseAuthorization();

app.MapControllers();

app.Run();
