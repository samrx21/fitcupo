namespace FitCupo.Identity.Application.Exceptions;

/// <summary>
/// Se lanza cuando los datos de la petición están incompletos o mal formados.
/// La capa de presentación la traduce a una respuesta 400 (Bad Request).
/// </summary>
public sealed class ValidationException(string message) : Exception(message);
