using FitCupo.Classes.Application.Utilities.Mediator;

namespace FitCupo.Classes.Application.UseCases.ClassSessions.Commands.ScheduleClassSession;

/// <summary>
/// Comando para programar una sesión. El instructor es el usuario autenticado:
/// su Id y su nombre vienen del JWT, no del cuerpo de la petición.
/// </summary>
public sealed record ScheduleClassSessionCommand(
    Guid GymClassId,
    Guid InstructorId,
    string InstructorName,
    string Room,
    DateTimeOffset StartsAt,
    int? Capacity) : IRequest<Guid>;
