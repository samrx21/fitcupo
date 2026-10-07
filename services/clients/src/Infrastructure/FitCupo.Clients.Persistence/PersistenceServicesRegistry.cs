using FitCupo.Clients.Application.Contracts.Persistence;
using FitCupo.Clients.Application.Contracts.Repositories;
using FitCupo.Clients.Persistence.Repositories;
using FitCupo.Clients.Persistence.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FitCupo.Clients.Persistence;

public static class PersistenceServicesRegistry
{
    public const string CONNECTION_STRING_NAME = "DefaultConnection";

    /// <summary>
    /// Registra el DbContext, el Unit of Work y los repositorios. La cadena de conexión
    /// la define el presentador (API) en su configuración: persistencia sabe el "cómo", no el "dónde".
    /// </summary>
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString(CONNECTION_STRING_NAME)
            ?? throw new InvalidOperationException(
                $"La cadena de conexión '{CONNECTION_STRING_NAME}' no está configurada.");

        services.AddDbContext<ClientsDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IClientRepository, ClientRepository>();

        return services;
    }

    /// <summary>
    /// Aplica las migraciones pendientes al iniciar la API, para que la base de datos
    /// se cree sola al ejecutar el proyecto por primera vez.
    /// </summary>
    public static async Task ApplyMigrationsAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using IServiceScope scope = services.CreateScope();
        ClientsDbContext context = scope.ServiceProvider.GetRequiredService<ClientsDbContext>();
        await context.Database.MigrateAsync(cancellationToken);
    }
}
