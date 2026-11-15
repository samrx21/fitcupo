namespace FitCupo.Classes.Application.Exceptions;

/// <summary>
/// Se lanza cuando el usuario autenticado no puede actuar sobre un recurso concreto,
/// por ejemplo, un instructor que intenta cancelar la sesión de otro instructor.
/// La capa de presentación la traduce a una respuesta 403 (Forbidden).
/// </summary>
public sealed class ForbiddenException(string message) : Exception(message);
