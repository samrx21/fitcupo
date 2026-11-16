using FitCupo.Identity.Application.Models;

namespace FitCupo.Identity.Application.Contracts.Identity;

/// <summary>
/// Operaciones sobre los usuarios. Se implementa en Infraestructura con ASP.NET Core Identity.
/// </summary>
public interface IIdentityService
{
    /// <summary>
    /// Verifica el correo y la contraseña. Si son válidos devuelve el usuario con sus roles;
    /// si no, lanza <see cref="Exceptions.InvalidCredentialsException"/>.
    /// </summary>
    Task<UserAccount> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);

    Task<UserAccount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UserAccount>> GetListAsync(string? role, CancellationToken cancellationToken = default);
}
