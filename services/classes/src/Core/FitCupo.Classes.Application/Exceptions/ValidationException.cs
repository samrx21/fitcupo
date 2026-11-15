namespace FitCupo.Classes.Application.Exceptions;

/// <summary>
/// Se lanza cuando los parámetros de una consulta no son coherentes, por ejemplo,
/// un rango de fechas invertido. La capa de presentación la traduce a 400 (Bad Request).
/// </summary>
public sealed class ValidationException(string message) : Exception(message);
