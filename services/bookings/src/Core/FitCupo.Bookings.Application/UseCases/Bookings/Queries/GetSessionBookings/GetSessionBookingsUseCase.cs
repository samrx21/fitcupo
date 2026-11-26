using FitCupo.Bookings.Application.Contracts.Repositories;
using FitCupo.Bookings.Application.Contracts.Services;
using FitCupo.Bookings.Application.Exceptions;
using FitCupo.Bookings.Application.Models;
using FitCupo.Bookings.Application.Utilities.Mediator;
using FitCupo.Bookings.Domain.Entities.Bookings;

namespace FitCupo.Bookings.Application.UseCases.Bookings.Queries.GetSessionBookings;

/// <summary>
/// Consulta a Clases los datos actuales de la sesión (quién la dicta y si sigue programada)
/// y los une con las reservas confirmadas que guarda este microservicio.
/// </summary>
public sealed class GetSessionBookingsUseCase(IClassesService classesService, IBookingRepository bookingRepository)
    : IRequestHandler<GetSessionBookingsQuery, SessionBookingsDto>
{
    public async Task<SessionBookingsDto> Handle(GetSessionBookingsQuery request, CancellationToken cancellationToken)
    {
        ClassSessionInfo session = await classesService.GetSessionAsync(request.SessionId, cancellationToken)
            ?? throw new NotFoundException($"No existe una sesión con el Id {request.SessionId}.");

        if (!request.CanViewAnySession && session.InstructorId != request.RequesterId)
            throw new ForbiddenException("Solo puedes ver los inscritos de las sesiones que tú dictas.");

        IReadOnlyList<Booking> bookings = await bookingRepository.GetConfirmedBySessionAsync(session.Id, cancellationToken);

        return new SessionBookingsDto
        {
            SessionId = session.Id,
            GymClassName = session.GymClassName,
            Room = session.Room,
            InstructorName = session.InstructorName,
            StartsAt = session.StartsAt,
            EndsAt = session.EndsAt,
            SessionStatus = session.Status,
            Capacity = session.Capacity,
            ReservedSpots = bookings.Count,
            AvailableSpots = Math.Max(session.Capacity - bookings.Count, 0),
            Bookings = bookings.Select(booking => booking.ToDto()).ToList()
        };
    }
}
