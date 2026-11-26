using FitCupo.Bookings.Application.Utilities.Mediator;

namespace FitCupo.Bookings.Application.UseCases.Bookings.Queries.GetBookingById;

/// <summary>
/// Query del detalle de una reserva. Un afiliado solo puede ver las suyas.
/// </summary>
public sealed record GetBookingByIdQuery(Guid Id, string RequesterEmail, bool CanViewAnyBooking) : IRequest<BookingDto>;
