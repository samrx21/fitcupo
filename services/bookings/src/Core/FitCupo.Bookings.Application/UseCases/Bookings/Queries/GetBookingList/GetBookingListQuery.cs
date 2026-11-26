using FitCupo.Bookings.Application.Utilities.Mediator;
using FitCupo.Bookings.Application.Utilities.Pagination;
using FitCupo.Bookings.Domain.Entities.Bookings;

namespace FitCupo.Bookings.Application.UseCases.Bookings.Queries.GetBookingList;

/// <summary>
/// Listado de todas las reservas para recepción y el administrador, con filtros opcionales.
/// </summary>
public sealed record GetBookingListQuery : IRequest<PaginationResponse<BookingDto>>
{
    public PaginationRequest Pagination { get; init; } = PaginationRequest.Standard();
    public Guid? ClientId { get; init; }
    public Guid? SessionId { get; init; }
    public BookingStatus? Status { get; init; }
    public bool UpcomingOnly { get; init; }
}
