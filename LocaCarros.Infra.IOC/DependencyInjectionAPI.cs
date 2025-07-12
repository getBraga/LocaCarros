using LocaCarros.Application.Interfaces;
using LocaCarros.Application.Mappings;
using LocaCarros.Application.Services;
using LocaCarros.Domain.Account;
using LocaCarros.Domain.Interfaces;
using LocaCarros.Infra.Data.Context;
using LocaCarros.Infra.Data.Identity;
using LocaCarros.Infra.Data.Repositories;
using LocaCarros.Infra.Data.Transaction;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LocaCarros.Infra.IOC
{
    public static class DependencyInjectionAPI
    {
        public static IServiceCollection AddInfrastructureAPI(this IServiceCollection services, 
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"
            ), b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             .AddJwtBearer(options =>
             {
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = new SymmetricSecurityKey(
                         Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]!)),
                     ValidateIssuer = true,
                     ValidIssuer = configuration["JWT:Issuer"],
                     ValidateAudience = true,
                     ValidAudience = configuration["JWT:Audience"],
                     ValidateLifetime = true,
                     ClockSkew = TimeSpan.Zero

                 };
                 options.Events = new JwtBearerEvents
                 {
                     OnAuthenticationFailed = context =>
                     {
                         if (context.Exception is SecurityTokenExpiredException)
                         {

                             context.Response.Headers.Append("Token-Expired", "true");
                         }
                         return Task.CompletedTask;
                     }
                 };
             });
                        services.AddScoped<IAuthenticate, AuthenticateService>();
            services.AddScoped<ISeedUserRoleInitial, SeedUserRoleInitial>();
            services.AddScoped<IAuthenticateServiceApplication, AuthenticateServiceApplication>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IMarcaRepository, MarcaRepository>();
            services.AddScoped<IModeloRepository, ModeloRepository>();
            services.AddScoped<ICarroRepository, CarroRepository>();
            services.AddScoped<IAluguelRepository, AluguelRepository>();
            services.AddScoped<IVendaRepository, VendaRepository>();
            services.AddScoped<IMarcaService, MarcaService>();
            services.AddScoped<IModeloService, ModeloService>();
            services.AddScoped<ICarroService, CarroService>();
            services.AddScoped<IVendaService, VendaService>();
            services.AddScoped<IAluguelService, AluguelService>();
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<DomainToDTOMappingProfile>();
            });
            return services;
        }

    }
}
