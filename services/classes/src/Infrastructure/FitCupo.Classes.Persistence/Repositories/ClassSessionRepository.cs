using FitCupo.Classes.Application.Contracts.Repositories;
using FitCupo.Classes.Application.UseCases.ClassSessions;
using FitCupo.Classes.Application.Utilities.Pagination;
using FitCupo.Classes.Domain.Entities.ClassSessions;
using FitCupo.Classes.Domain.Entities.ClassSessions.ValueObjects;
using FitCupo.Classes.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FitCupo.Classes.Persistence.Repositories;

public sealed class ClassSessionRepository(ClassesDbContext context)
    : Repository<ClassSession>(context), IClassSessionRepository
{
    public async Task<bool> IsRoomBusyAsync(
        string room, TimeSlot schedule, CancellationToken cancellationToken = default) =>
        await ScheduledSessionsOverlapping(schedule)
            .AnyAsync(session => session.Room == room, cancellationToken);

    public async Task<bool> IsInstructorBusyAsync(
        Guid instructorId, TimeSlot schedule, CancellationToken cancellationToken = default) =>
        await ScheduledSessionsOverlapping(schedule)
            .AnyAsync(session => session.InstructorId == instructorId, cancellationToken);

    public async Task<ClassSessionDto?> GetDtoByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await ToDto(Context.ClassSessions.AsNoTracking().Where(session => session.Id == id))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<PaginationResponse<ClassSessionDto>> GetScheduleAsync(
        PaginationRequest pagination,
        DateTime from,
        DateTime to,
        Guid? gymClassId,
        Guid? instructorId,
        ClassSessionStatus? status,
        CancellationToken cancellationToken = default)
    {
        IQueryable<ClassSession> query = Context.ClassSessions
            .AsNoTracking()
            .Where(session => session.TimeSlot.StartsAt >= from && session.TimeSlot.StartsAt <= to);

        if (gymClassId.HasValue)
            query = query.Where(session => session.GymClassId == gymClassId.Value);

        if (instructorId.HasValue)
            query = query.Where(session => session.InstructorId == instructorId.Value);

        if (status.HasValue)
            query = query.Where(session => session.Status == status.Value);

        query = query.OrderBy(session => session.TimeSlot.StartsAt).ThenBy(session => session.Id);

        return await ToDto(query).ToPagedListAsync(pagination, cancellationToken);
    }

    /// <summary>
    /// Sesiones programadas (no canceladas) cuya franja se cruza con la indicada.
    /// </summary>
    private IQueryable<ClassSession> ScheduledSessionsOverlapping(TimeSlot schedule) =>
        Context.ClassSessions.Where(session =>
            session.Status == ClassSessionStatus.Scheduled
            && session.TimeSlot.StartsAt < schedule.EndsAt
            && schedule.StartsAt < session.TimeSlot.EndsAt);

    /// <summary>
    /// Proyecta las sesiones al DTO del horario uniéndolas con su clase, en una sola consulta SQL.
    /// </summary>
    private IQueryable<ClassSessionDto> ToDto(IQueryable<ClassSession> sessions) =>
        from session in sessions
        join gymClass in Context.GymClasses on session.GymClassId equals gymClass.Id
        select new ClassSessionDto
        {
            Id = session.Id,
            GymClassId = gymClass.Id,
            GymClassName = gymClass.Name,
            Category = gymClass.Category,
            InstructorId = session.InstructorId,
            InstructorName = session.InstructorName,
            Room = session.Room,
            StartsAt = session.TimeSlot.StartsAt,
            EndsAt = session.TimeSlot.EndsAt,
            Capacity = session.Capacity,
            Status = session.Status,
            CancellationReason = session.CancellationReason
        };
}
