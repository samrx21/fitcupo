namespace FitCupo.Bookings.API.Security;

/// <summary>
/// Nombres de los claims que Identity incluye en el JWT.
/// </summary>
public static class TokenClaims
{
    public const string SUBJECT = "sub";
    public const string EMAIL = "email";
    public const string NAME = "name";
    public const string ROLE = "role";
}
