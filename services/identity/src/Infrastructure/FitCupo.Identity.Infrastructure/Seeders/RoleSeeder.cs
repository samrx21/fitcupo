using FitCupo.Identity.Application.Constants;
using FitCupo.Identity.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FitCupo.Identity.Infrastructure.Seeders;

/// <summary>
/// Crea los roles iniciales de FitCupo si todavía no existen.
/// </summary>
public sealed class RoleSeeder(RoleManager<ApplicationRole> roleManager, ILogger<RoleSeeder> logger)
{
    private static readonly (string Name, string Description)[] InitialRoles =
    [
        (Roles.ADMIN, "Administra las clases, los clientes y los usuarios del gimnasio"),
        (Roles.RECEPTIONIST, "Registra y actualiza los clientes en la recepción"),
        (Roles.INSTRUCTOR, "Programa y cancela las sesiones de sus clases"),
        (Roles.MEMBER, "Afiliado: consulta el horario de clases y su perfil")
    ];

    public async Task SeedAsync()
    {
        foreach ((string name, string description) in InitialRoles)
        {
            if (await roleManager.RoleExistsAsync(name))
            {
                logger.LogInformation("Seeder: el rol {Role} ya existe.", name);
                continue;
            }

            IdentityResult result = await roleManager.CreateAsync(new ApplicationRole { Name = name, Description = description });
            SeederResult.EnsureSucceeded(result, $"crear el rol {name}");

            logger.LogInformation("Seeder: rol {Role} creado.", name);
        }
    }
}
