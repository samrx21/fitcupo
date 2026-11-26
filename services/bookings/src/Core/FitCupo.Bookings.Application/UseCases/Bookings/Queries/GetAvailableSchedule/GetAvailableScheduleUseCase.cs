using FitCupo.Bookings.Application.Contracts.Repositories;
using FitCupo.Bookings.Application.Contracts.Services;
using FitCupo.Bookings.Application.Exceptions;
using FitCupo.Bookings.Application.Models;
using FitCupo.Bookings.Application.Utilities.Mediator;

namespace FitCupo.Bookings.Application.UseCases.Bookings.Queries.GetAvailableSchedule;

/// <summary>
/// Pide el horario al microservicio de Clases (llamada síncrona) y le agrega los cupos
/// ocupados que guarda Reservas, con una sola consulta a la base de datos.
/// </summary>
public sealed class GetAvailableScheduleUseCase(IClassesService classesService, IBookingRepository bookingRepository)
    : IRequestHandler<GetAvailableScheduleQuery, IReadOnlyList<AvailableSessionDto>>
{
    public async Task<IReadOnlyList<AvailableSessionDto>> Handle(
        GetAvailableScheduleQuery request, CancellationToken cancellationToken)
    {
        DateTimeOffset from = request.From ?? DateTimeOffset.UtcNow;
        DateTimeOffset to = request.To ?? from.AddDays(GetAvailableScheduleQuery.DEFAULT_RANGE_DAYS);

        if (from > to)
            throw new ValidationException("La fecha inicial no puede ser posterior a la fecha final.");

        if (to - from > TimeSpan.FromDays(GetAvailableScheduleQuery.MAX_RANGE_DAYS))
            throw new ValidationException(
                $"El rango del horario no puede superar {GetAvailableScheduleQuery.MAX_RANGE_DAYS} días.");

        IReadOnlyList<ClassSessionInfo> sessions = await classesService.GetScheduleAsync(from, to, cancellationToken);
        if (sessions.Count == 0)
            return [];

        Guid[] sessionIds = sessions.Select(session => session.Id).ToArray();
        IReadOnlyDictionary<Guid, int> reserved =
            await bookingRepository.CountConfirmedBySessionsAsync(sessionIds, cancellationToken);

        IReadOnlyDictionary<Guid, Guid> myBookings = string.IsNullOrWhiteSpace(request.RequesterEmail)
            ? new Dictionary<Guid, Guid>()
            : await bookingRepository.GetConfirmedBookingIdsAsync(
                request.RequesterEmail.Trim().ToLowerInvariant(), sessionIds, cancellationToken);

        return sessions
            .Select(session =>
            {
                int reservedSpots = reserved.GetValueOrDefault(session.Id);
                return new AvailableSessionDto
                {
                    SessionId = session.Id,
                    GymClassId = session.GymClassId,
                    GymClassName = session.GymClassName,
                    Category = session.Category,
                    InstructorId = session.InstructorId,
                    InstructorName = session.InstructorName,
                    Room = session.Room,
                    StartsAt = session.StartsAt,
                    EndsAt = session.EndsAt,
                    Status = session.Status,
                    CancellationReason = session.CancellationReason,
                    Capacity = session.Capacity,
                    ReservedSpots = reservedSpots,
                    AvailableSpots = session.IsScheduled ? Math.Max(session.Capacity - reservedSpots, 0) : 0,
                    MyBookingId = myBookings.TryGetValue(session.Id, out Guid bookingId) ? bookingId : null
                };
            })
            .ToList();
    }
}
