using System.Security.Claims;

namespace FitCupo.Clients.API.Security;

/// <summary>
/// Lee los datos del usuario autenticado a partir de los claims del JWT.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    public static string GetEmail(this ClaimsPrincipal user) =>
        user.FindFirstValue(TokenClaims.EMAIL)
        ?? throw new InvalidOperationException("El token no contiene el correo del usuario.");
}
