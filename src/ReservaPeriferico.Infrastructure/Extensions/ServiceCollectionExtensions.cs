using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReservaPeriferico.Application.Interfaces;
using ReservaPeriferico.Application.Services;
using ReservaPeriferico.Core.Interfaces;
using ReservaPeriferico.Infrastructure.Data;
using ReservaPeriferico.Infrastructure.Repositories;

namespace ReservaPeriferico.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configurar Entity Framework com PostgreSQL
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Registrar repositórios
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IPerifericoRepository, PerifericoRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IReservaRepository, ReservaRepository>();

        // Registrar serviços de aplicação
        services.AddScoped<IPerifericoService, PerifericoService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IReservaService, ReservaService>();

        return services;
    }
} 