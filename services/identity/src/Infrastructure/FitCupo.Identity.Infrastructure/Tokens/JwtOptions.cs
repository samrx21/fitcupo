namespace FitCupo.Identity.Infrastructure.Tokens;

/// <summary>
/// Configuración del JWT (sección "Jwt" de appsettings). Los demás microservicios
/// usan el mismo emisor, audiencia y clave para validar los tokens.
/// </summary>
public sealed class JwtOptions
{
    public const string SECTION_NAME = "Jwt";
    public const int MIN_SIGNING_KEY_LENGTH = 32;

    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public string SigningKey { get; init; } = null!;
    public int ExpirationMinutes { get; init; } = 60;
}
