using System.Security.Claims;

namespace FitCupo.Bookings.API.Security;

/// <summary>
/// Lee los datos del usuario autenticado a partir de los claims del JWT.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user) =>
        Guid.TryParse(user.FindFirstValue(TokenClaims.SUBJECT), out Guid id)
            ? id
            : throw new InvalidOperationException("El token no contiene el Id del usuario.");

    public static string GetEmail(this ClaimsPrincipal user) =>
        user.FindFirstValue(TokenClaims.EMAIL)
        ?? throw new InvalidOperationException("El token no contiene el correo del usuario.");

    /// <summary>
    /// Recepción y el administrador gestionan las reservas de cualquier cliente.
    /// </summary>
    public static bool IsStaff(this ClaimsPrincipal user) =>
        user.IsInRole(Roles.ADMIN) || user.IsInRole(Roles.RECEPTIONIST);
}
