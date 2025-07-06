using LocaCarros.Application.Interfaces;
using LocaCarros.Application.Mappings;
using LocaCarros.Application.Services;
using LocaCarros.Domain.Interfaces;
using LocaCarros.Infra.Data.Context;
using LocaCarros.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LocaCarros.Infra.IOC
{
    public static class DependencyInjectionAPI
    {
        public static IServiceCollection AddInfrastructureAPI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"
            ), b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddScoped<IMarcaRepository, MarcaRepository>();
            services.AddScoped<IModeloRepository, ModeloRepository>();
            services.AddScoped<ICarroRepository, CarroRepository>();
            services.AddScoped<IAluguelRepository, AluguelRepository>();
            services.AddScoped<IVendaRepository, VendaRepository>();
            services.AddScoped<IMarcaService, MarcaService>();
            services.AddScoped<IModeloService, ModeloService>();
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<DomainToDTOMappingProfile>();
            });
            return services;
        }

    }
}
