namespace FitCupo.Bookings.Application.Exceptions;

/// <summary>
/// Se lanza cuando la operación choca con datos que ya existen,
/// por ejemplo, un cliente que ya reservó la misma sesión.
/// La capa de presentación la traduce a una respuesta 409 (Conflict).
/// </summary>
public sealed class ConflictException(string message) : Exception(message);
