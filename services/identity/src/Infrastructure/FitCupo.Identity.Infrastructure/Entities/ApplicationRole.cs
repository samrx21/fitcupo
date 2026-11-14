using Microsoft.AspNetCore.Identity;

namespace FitCupo.Identity.Infrastructure.Entities;

/// <summary>
/// Rol de FitCupo (Admin, Receptionist, Instructor o Member) con una descripción legible.
/// </summary>
public sealed class ApplicationRole : IdentityRole<Guid>
{
    public const int DESCRIPTION_MAX_LENGTH = 200;

    public string Description { get; set; } = null!;

    public ICollection<ApplicationUserRole> UserRoles { get; set; } = [];

    public ApplicationRole()
    {
        Id = Guid.CreateVersion7();
    }
}
