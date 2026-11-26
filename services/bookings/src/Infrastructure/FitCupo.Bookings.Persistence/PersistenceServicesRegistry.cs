using FitCupo.Bookings.Application.Contracts.Persistence;
using FitCupo.Bookings.Application.Contracts.Repositories;
using FitCupo.Bookings.Persistence.Repositories;
using FitCupo.Bookings.Persistence.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FitCupo.Bookings.Persistence;

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

        services.AddDbContext<BookingsDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IBookingRepository, BookingRepository>();

        return services;
    }

    /// <summary>
    /// Aplica las migraciones pendientes al iniciar la API, para que la base de datos
    /// se cree sola al ejecutar el proyecto por primera vez.
    /// </summary>
    public static async Task ApplyMigrationsAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using IServiceScope scope = services.CreateScope();
        BookingsDbContext context = scope.ServiceProvider.GetRequiredService<BookingsDbContext>();
        await context.Database.MigrateAsync(cancellationToken);
    }
}
