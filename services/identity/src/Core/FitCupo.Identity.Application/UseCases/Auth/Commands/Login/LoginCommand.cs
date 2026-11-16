using FitCupo.Identity.Application.Utilities.Mediator;

namespace FitCupo.Identity.Application.UseCases.Auth.Commands.Login;

/// <summary>
/// Comando para iniciar sesión con correo y contraseña. Devuelve el JWT del usuario.
/// </summary>
public sealed record LoginCommand(string Email, string Password) : IRequest<LoginResponseDto>;
