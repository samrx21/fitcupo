using FitCupo.Identity.Application.UseCases.Users;

namespace FitCupo.Identity.Application.UseCases.Auth.Commands.Login;

/// <summary>
/// Respuesta de un inicio de sesión exitoso. El cliente envía <see cref="AccessToken"/>
/// en el encabezado <c>Authorization: Bearer</c> a los demás microservicios.
/// </summary>
public sealed class LoginResponseDto
{
    public string AccessToken { get; init; } = null!;
    public string TokenType { get; init; } = "Bearer";
    public DateTime ExpiresAt { get; init; }
    public UserDto User { get; init; } = null!;
}
