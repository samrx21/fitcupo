namespace FitCupo.Identity.Application.Exceptions;

/// <summary>
/// Se lanza cuando el correo o la contraseña no son válidos, o la cuenta está bloqueada.
/// La capa de presentación la traduce a una respuesta 401 (Unauthorized).
/// </summary>
public sealed class InvalidCredentialsException(string message) : Exception(message);
