using FitCupo.Classes.Application.Contracts.Persistence;
using FitCupo.Classes.Application.Contracts.Repositories;
using FitCupo.Classes.Application.Exceptions;
using FitCupo.Classes.Application.Utilities.Mediator;
using FitCupo.Classes.Domain.Entities.ClassSessions;
using FitCupo.Classes.Domain.Entities.GymClasses;

namespace FitCupo.Classes.Application.UseCases.ClassSessions.Commands.ScheduleClassSession;

public sealed class ScheduleClassSessionUseCase(
    IGymClassRepository gymClassRepository,
    IClassSessionRepository classSessionRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ScheduleClassSessionCommand, Guid>
{
    public async Task<Guid> Handle(ScheduleClassSessionCommand request, CancellationToken cancellationToken)
    {
        GymClass gymClass = await gymClassRepository.GetByIdAsync(request.GymClassId, cancellationToken)
            ?? throw new NotFoundException($"No existe una clase con el Id {request.GymClassId}.");

        // El dominio valida la clase, la fecha, el salón y el cupo, y calcula la hora de fin
        ClassSession session = ClassSession.Schedule(
            gymClass,
            request.InstructorId,
            request.InstructorName,
            request.Room,
            request.StartsAt.UtcDateTime,
            request.Capacity);

        // Los cruces de horario dependen de las sesiones guardadas, por eso se validan aquí
        if (await classSessionRepository.IsRoomBusyAsync(session.Room, session.TimeSlot, cancellationToken))
            throw new ConflictException($"El salón {session.Room} ya tiene una sesión en ese horario.");

        if (await classSessionRepository.IsInstructorBusyAsync(session.InstructorId, session.TimeSlot, cancellationToken))
            throw new ConflictException("Ya tienes otra sesión programada en ese horario.");

        await classSessionRepository.AddAsync(session, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return session.Id;
    }
}
