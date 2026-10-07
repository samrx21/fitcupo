namespace FitCupo.Clients.Application.Exceptions;

/// <summary>
/// Se lanza cuando el mediador no encuentra un caso de uso para una petición.
/// Indica un error de configuración, no un error del usuario.
/// </summary>
public sealed class MediatorException(string message) : Exception(message);
