namespace FitCupo.Clients.Application.Exceptions;

/// <summary>
/// Se lanza cuando el recurso solicitado no existe.
/// La capa de presentación la traduce a una respuesta 404 (Not Found).
/// </summary>
public sealed class NotFoundException(string message) : Exception(message);
