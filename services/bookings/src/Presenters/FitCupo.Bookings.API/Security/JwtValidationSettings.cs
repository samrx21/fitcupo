using System.Text;

namespace FitCupo.Bookings.API.Security;

/// <summary>
/// Datos para validar el JWT (sección "Jwt" de appsettings). Deben coincidir con los de Identity.
/// </summary>
public sealed class JwtValidationSettings
{
    public const int MIN_SIGNING_KEY_BYTES = 32;

    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public string SigningKey { get; init; } = null!;

    public void EnsureIsValid()
    {
        if (string.IsNullOrWhiteSpace(Issuer) || string.IsNullOrWhiteSpace(Audience))
            throw new InvalidOperationException("Jwt:Issuer y Jwt:Audience son requeridos.");

        // HMAC-SHA256 necesita una clave de al menos 256 bits
        if (string.IsNullOrEmpty(SigningKey) || Encoding.UTF8.GetByteCount(SigningKey) < MIN_SIGNING_KEY_BYTES)
            throw new InvalidOperationException(
                $"Jwt:SigningKey debe tener al menos {MIN_SIGNING_KEY_BYTES} bytes.");
    }
}
