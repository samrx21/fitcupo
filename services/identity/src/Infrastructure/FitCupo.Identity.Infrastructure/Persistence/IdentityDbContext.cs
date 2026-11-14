using FitCupo.Identity.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitCupo.Identity.Infrastructure.Persistence;

/// <summary>
/// Contexto de ASP.NET Core Identity con las entidades propias de FitCupo.
/// Crea las tablas de usuarios, roles y la relación entre ambos.
/// </summary>
public sealed class FitCupoIdentityDbContext(DbContextOptions<FitCupoIdentityDbContext> options)
    : IdentityDbContext<
        ApplicationUser,
        ApplicationRole,
        Guid,
        IdentityUserClaim<Guid>,
        ApplicationUserRole,
        IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>,
        IdentityUserToken<Guid>>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Primero Identity configura sus tablas; luego se aplican los ajustes de la carpeta Configurations
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FitCupoIdentityDbContext).Assembly);
    }
}
