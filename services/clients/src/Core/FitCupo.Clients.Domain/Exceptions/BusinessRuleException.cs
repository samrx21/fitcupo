namespace FitCupo.Clients.Domain.Exceptions;

/// <summary>
/// Excepción que se lanza cuando se incumple una regla de negocio del dominio.
/// La capa de presentación la traduce a una respuesta 400 (Bad Request).
/// </summary>
public sealed class BusinessRuleException(string message) : Exception(message);
