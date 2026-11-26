namespace FitCupo.Bookings.Application.Exceptions;

/// <summary>
/// Se lanza cuando otro microservicio (Clases o Clientes) no responde o falla.
/// Reservas depende de ellos para validar cada reserva, así que la operación no puede continuar.
/// La capa de presentación la traduce a una respuesta 503 (Service Unavailable).
/// </summary>
public sealed class ServiceUnavailableException(string message) : Exception(message);
