using FitCupo.Classes.Application.Contracts.Repositories;
using FitCupo.Classes.Application.Exceptions;
using FitCupo.Classes.Application.Utilities.Mediator;
using FitCupo.Classes.Application.Utilities.Pagination;

namespace FitCupo.Classes.Application.UseCases.ClassSessions.Queries.GetClassSchedule;

public sealed class GetClassScheduleUseCase(IClassSessionRepository classSessionRepository)
    : IRequestHandler<GetClassScheduleQuery, PaginationResponse<ClassSessionDto>>
{
    public async Task<PaginationResponse<ClassSessionDto>> Handle(
        GetClassScheduleQuery request, CancellationToken cancellationToken)
    {
        DateTime from = request.From?.UtcDateTime ?? DateTime.UtcNow;
        DateTime to = request.To?.UtcDateTime ?? from.AddDays(GetClassScheduleQuery.DEFAULT_RANGE_DAYS);

        if (from > to)
            throw new ValidationException("La fecha inicial no puede ser posterior a la fecha final.");

        return await classSessionRepository.GetScheduleAsync(
            request.Pagination,
            from,
            to,
            request.GymClassId,
            request.InstructorId,
            request.Status,
            cancellationToken);
    }
}
