using FitCupo.Bookings.Application.Utilities.Mediator;

namespace FitCupo.Bookings.Application.UseCases.Bookings.Queries.GetSessionBookings;

/// <summary>
/// Inscritos de una sesión. <paramref name="CanViewAnySession"/> es verdadero para recepción y el
/// administrador; un instructor solo ve los inscritos de las sesiones que dicta.
/// </summary>
public sealed record GetSessionBookingsQuery(Guid SessionId, Guid RequesterId, bool CanViewAnySession)
    : IRequest<SessionBookingsDto>;
