using FitCupo.Bookings.Application.Utilities.Mediator;
using FitCupo.Bookings.Application.Utilities.Pagination;
using FitCupo.Bookings.Domain.Entities.Bookings;

namespace FitCupo.Bookings.Application.UseCases.Bookings.Queries.GetMyBookings;

/// <summary>
/// Reservas del afiliado que inició sesión. Se buscan con el correo de su token.
/// </summary>
public sealed record GetMyBookingsQuery(string Email) : IRequest<PaginationResponse<BookingDto>>
{
    public PaginationRequest Pagination { get; init; } = PaginationRequest.Standard();
    public BookingStatus? Status { get; init; }

    /// <summary>
    /// Si es verdadero, solo devuelve las reservas de sesiones que todavía no terminan.
    /// </summary>
    public bool UpcomingOnly { get; init; }
}
