using FitCupo.Identity.Infrastructure.Entities;
using FitCupo.Identity.Infrastructure.Persistence;
using FitCupo.Identity.Infrastructure.Seeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FitCupo.Identity.Infrastructure;

public static class InfrastructureServicesRegistry
{
    public const string CONNECTION_STRING_NAME = "DefaultConnection";

    /// <summary>
    /// Registra el DbContext, ASP.NET Core Identity con sus reglas de contraseña y bloqueo,
    /// y los seeders.
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString(CONNECTION_STRING_NAME)
            ?? throw new InvalidOperationException(
                $"La cadena de conexión '{CONNECTION_STRING_NAME}' no está configurada.");

        services.AddDbContext<FitCupoIdentityDbContext>(options => options.UseSqlServer(connectionString));

        // AddIdentityCore no registra cookies: la API autentica solo con JWT
        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<FitCupoIdentityDbContext>()
            .AddSignInManager();

        services.AddScoped<RoleSeeder>();
        services.AddScoped<UserSeeder>();

        return services;
    }

    /// <summary>
    /// Aplica las migraciones pendientes y ejecuta los seeders de roles y usuarios.
    /// Los seeders no duplican datos: solo crean lo que todavía no existe.
    /// </summary>
    public static async Task InitializeDatabaseAsync(this IServiceProvider services)
    {
        using IServiceScope scope = services.CreateScope();

        FitCupoIdentityDbContext context = scope.ServiceProvider.GetRequiredService<FitCupoIdentityDbContext>();
        await context.Database.MigrateAsync();

        await scope.ServiceProvider.GetRequiredService<RoleSeeder>().SeedAsync();
        await scope.ServiceProvider.GetRequiredService<UserSeeder>().SeedAsync();
    }
}
