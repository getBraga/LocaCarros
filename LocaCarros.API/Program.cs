
using LocaCarros.Application.Mappings;
using LocaCarros.Infra.IOC;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddInfrastructureAPI(builder.Configuration);
//builder.Services.AddEndpointsApiExplorer(); // Habilita a geração de endpoints para Swagger
builder.Services.AddInfrastructureSwagger(); 
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();


app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "LocaCarros.API v1");
    c.RoutePrefix = string.Empty; // 👈 faz o Swagger ser carregado na raiz
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
