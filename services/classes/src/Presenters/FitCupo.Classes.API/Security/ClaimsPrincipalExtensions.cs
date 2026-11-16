using System.Security.Claims;

namespace FitCupo.Classes.API.Security;

/// <summary>
/// Lee los datos del usuario autenticado a partir de los claims del JWT.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user) =>
        Guid.TryParse(user.FindFirstValue(TokenClaims.SUBJECT), out Guid id)
            ? id
            : throw new InvalidOperationException("El token no contiene el Id del usuario.");

    public static string GetFullName(this ClaimsPrincipal user) =>
        user.FindFirstValue(TokenClaims.NAME)
        ?? throw new InvalidOperationException("El token no contiene el nombre del usuario.");
}
