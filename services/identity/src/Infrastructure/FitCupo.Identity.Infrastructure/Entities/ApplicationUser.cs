using Microsoft.AspNetCore.Identity;

namespace FitCupo.Identity.Infrastructure.Entities;

/// <summary>
/// Usuario de FitCupo. Hereda de IdentityUser el correo, el hash de la contraseña,
/// el control de bloqueo y el sello de seguridad; aquí solo se agregan los nombres.
/// </summary>
public sealed class ApplicationUser : IdentityUser<Guid>
{
    public const int NAME_MAX_LENGTH = 64;

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Relación con los roles a través de la tabla intermedia UserRoles.
    /// </summary>
    public ICollection<ApplicationUserRole> UserRoles { get; set; } = [];

    public ApplicationUser()
    {
        Id = Guid.CreateVersion7();
        CreatedAt = DateTime.UtcNow;
    }
}
