using Microsoft.AspNetCore.Identity;

namespace FitCupo.Identity.Infrastructure.Entities;

/// <summary>
/// Tabla intermedia de la relación muchos a muchos entre usuarios y roles.
/// Se declara para poder navegar de un usuario a sus roles en las consultas.
/// </summary>
public sealed class ApplicationUserRole : IdentityUserRole<Guid>
{
    public ApplicationUser User { get; set; } = null!;
    public ApplicationRole Role { get; set; } = null!;
}
