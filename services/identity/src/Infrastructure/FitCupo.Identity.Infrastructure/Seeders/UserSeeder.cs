using FitCupo.Identity.Application.Constants;
using FitCupo.Identity.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FitCupo.Identity.Infrastructure.Seeders;

/// <summary>
/// Crea los usuarios iniciales y les asigna su rol. UserManager guarda la contraseña
/// como hash (PBKDF2 con sal), nunca en texto plano.
/// </summary>
public sealed class UserSeeder(
    UserManager<ApplicationUser> userManager,
    IConfiguration configuration,
    ILogger<UserSeeder> logger)
{
    public const string PASSWORD_SETTING = "Seed:DefaultPassword";

    private static readonly (string Email, string FirstName, string LastName, string Role)[] InitialUsers =
    [
        ("admin@fitcupo.com", "Andrea", "Ruiz", Roles.ADMIN),
        ("recepcion@fitcupo.com", "Mateo", "Torres", Roles.RECEPTIONIST),
        ("instructor@fitcupo.com", "Carolina", "Ríos", Roles.INSTRUCTOR),
        ("instructor2@fitcupo.com", "Julián", "Pérez", Roles.INSTRUCTOR),
        ("afiliado@fitcupo.com", "Laura", "Gómez", Roles.MEMBER)
    ];

    public async Task SeedAsync()
    {
        // La contraseña inicial se lee de la configuración para no escribirla en el código
        string password = configuration[PASSWORD_SETTING]
            ?? throw new InvalidOperationException($"Falta la configuración {PASSWORD_SETTING}.");

        foreach ((string email, string firstName, string lastName, string role) in InitialUsers)
        {
            if (await userManager.FindByEmailAsync(email) is not null)
            {
                logger.LogInformation("Seeder: el usuario {Email} ya existe.", email);
                continue;
            }

            ApplicationUser user = new()
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FirstName = firstName,
                LastName = lastName
            };

            SeederResult.EnsureSucceeded(await userManager.CreateAsync(user, password), $"crear el usuario {email}");
            SeederResult.EnsureSucceeded(await userManager.AddToRoleAsync(user, role), $"asignar el rol {role} a {email}");

            logger.LogInformation("Seeder: usuario {Email} creado con el rol {Role}.", email, role);
        }
    }
}
