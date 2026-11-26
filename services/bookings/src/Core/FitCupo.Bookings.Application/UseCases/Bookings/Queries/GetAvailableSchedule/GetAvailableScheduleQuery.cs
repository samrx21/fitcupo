using FitCupo.Bookings.Application.Utilities.Mediator;

namespace FitCupo.Bookings.Application.UseCases.Bookings.Queries.GetAvailableSchedule;

/// <summary>
/// Horario con cupos disponibles. Sin fechas, devuelve los próximos 7 días.
/// <paramref name="RequesterEmail"/> se usa para marcar las sesiones que el afiliado ya reservó.
/// </summary>
public sealed record GetAvailableScheduleQuery(string? RequesterEmail) : IRequest<IReadOnlyList<AvailableSessionDto>>
{
    public const int DEFAULT_RANGE_DAYS = 7;
    public const int MAX_RANGE_DAYS = 31;

    public DateTimeOffset? From { get; init; }
    public DateTimeOffset? To { get; init; }
}
