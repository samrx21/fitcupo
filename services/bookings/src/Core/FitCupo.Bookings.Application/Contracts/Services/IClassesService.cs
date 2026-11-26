using FitCupo.Bookings.Application.Models;

namespace FitCupo.Bookings.Application.Contracts.Services;

/// <summary>
/// Comunicación síncrona (HTTP) con el microservicio de Clases. Se implementa en Infraestructura;
/// los casos de uso no saben que la información viene de otra API.
/// </summary>
public interface IClassesService
{
    /// <summary>
    /// Consulta una sesión por su Id. Devuelve null si Clases responde 404.
    /// </summary>
    Task<ClassSessionInfo?> GetSessionAsync(Guid sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Consulta el horario entre dos fechas (todas las páginas).
    /// </summary>
    Task<IReadOnlyList<ClassSessionInfo>> GetScheduleAsync(
        DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
}
