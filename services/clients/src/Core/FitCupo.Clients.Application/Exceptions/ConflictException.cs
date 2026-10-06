namespace FitCupo.Clients.Application.Exceptions;

/// <summary>
/// Se lanza cuando la operación choca con datos que ya existen,
/// por ejemplo un documento o un correo ya registrado.
/// La capa de presentación la traduce a una respuesta 409 (Conflict).
/// </summary>
public sealed class ConflictException(string message) : Exception(message);
