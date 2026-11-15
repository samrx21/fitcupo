using FitCupo.Classes.Application.Utilities.Mediator;
using FitCupo.Classes.Application.Utilities.Pagination;
using FitCupo.Classes.Domain.Entities.ClassSessions;

namespace FitCupo.Classes.Application.UseCases.ClassSessions.Queries.GetClassSchedule;

/// <summary>
/// Query del horario de clases. Sin fechas, devuelve las sesiones de los próximos 7 días.
/// </summary>
public sealed record GetClassScheduleQuery : IRequest<PaginationResponse<ClassSessionDto>>
{
    public const int DEFAULT_RANGE_DAYS = 7;

    public PaginationRequest Pagination { get; init; } = PaginationRequest.Standard();
    public DateTimeOffset? From { get; init; }
    public DateTimeOffset? To { get; init; }
    public Guid? GymClassId { get; init; }
    public Guid? InstructorId { get; init; }
    public ClassSessionStatus? Status { get; init; }
}
