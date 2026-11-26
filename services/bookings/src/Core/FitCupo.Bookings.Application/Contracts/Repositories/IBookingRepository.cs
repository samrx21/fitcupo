using FitCupo.Bookings.Application.Utilities.Pagination;
using FitCupo.Bookings.Domain.Entities.Bookings;

namespace FitCupo.Bookings.Application.Contracts.Repositories;

/// <summary>
/// Operaciones propias del repositorio de reservas.
/// </summary>
public interface IBookingRepository : IRepository<Booking>
{
    /// <summary>
    /// Número de reservas confirmadas de una sesión (cupos ocupados).
    /// </summary>
    Task<int> CountConfirmedBySessionAsync(Guid sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cupos ocupados de varias sesiones a la vez, en una sola consulta: Id de la sesión → reservas confirmadas.
    /// </summary>
    Task<IReadOnlyDictionary<Guid, int>> CountConfirmedBySessionsAsync(
        IReadOnlyCollection<Guid> sessionIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Indica si el cliente ya tiene una reserva confirmada en la sesión.
    /// </summary>
    Task<bool> HasConfirmedBookingAsync(Guid sessionId, Guid clientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Indica si el cliente tiene otra reserva confirmada que se cruza con la franja indicada.
    /// </summary>
    Task<bool> HasOverlappingBookingAsync(
        Guid clientId, DateTime startsAt, DateTime endsAt, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reservas confirmadas del afiliado (por su correo) en las sesiones indicadas: Id de la sesión → Id de la reserva.
    /// </summary>
    Task<IReadOnlyDictionary<Guid, Guid>> GetConfirmedBookingIdsAsync(
        string clientEmail, IReadOnlyCollection<Guid> sessionIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reservas confirmadas de una sesión, ordenadas por fecha de reserva.
    /// </summary>
    Task<IReadOnlyList<Booking>> GetConfirmedBySessionAsync(Guid sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Listado paginado con filtros, ordenado por la fecha de la sesión (las más próximas primero).
    /// </summary>
    Task<PaginationResponse<Booking>> GetPagedListAsync(
        PaginationRequest pagination,
        string? clientEmail,
        Guid? clientId,
        Guid? sessionId,
        BookingStatus? status,
        bool upcomingOnly,
        CancellationToken cancellationToken = default);
}
