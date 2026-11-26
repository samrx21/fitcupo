namespace FitCupo.Bookings.Application.Exceptions;

/// <summary>
/// Se lanza cuando el usuario autenticado no puede actuar sobre un recurso concreto,
/// por ejemplo, un afiliado que intenta cancelar la reserva de otra persona.
/// La capa de presentación la traduce a una respuesta 403 (Forbidden).
/// </summary>
public sealed class ForbiddenException(string message) : Exception(message);
