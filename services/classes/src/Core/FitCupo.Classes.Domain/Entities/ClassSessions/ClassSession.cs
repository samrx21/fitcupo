using FitCupo.Classes.Domain.Entities.ClassSessions.ValueObjects;
using FitCupo.Classes.Domain.Entities.GymClasses;
using FitCupo.Classes.Domain.Exceptions;

namespace FitCupo.Classes.Domain.Entities.ClassSessions;

/// <summary>
/// Sesión programada de una clase: fecha, salón, instructor y cupos. Es un agregado
/// distinto de GymClass y la referencia solo por su Id.
/// </summary>
public sealed class ClassSession
{
    public const int ROOM_MIN_LENGTH = 2;
    public const int ROOM_MAX_LENGTH = 40;
    public const int INSTRUCTOR_NAME_MAX_LENGTH = 130;
    public const int REASON_MIN_LENGTH = 5;
    public const int REASON_MAX_LENGTH = 200;
    public const int MAX_DAYS_IN_ADVANCE = 60;

    public Guid Id { get; private set; }
    public Guid GymClassId { get; private set; }

    /// <summary>
    /// Id del usuario de Identity que dicta la sesión (claim "sub" del JWT).
    /// </summary>
    public Guid InstructorId { get; private set; }
    public string InstructorName { get; private set; } = null!;
    public string Room { get; private set; } = null!;
    public TimeSlot TimeSlot { get; private set; } = null!;
    public int Capacity { get; private set; }
    public ClassSessionStatus Status { get; private set; }
    public string? CancellationReason { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }

    // Requerido por EF Core para materializar la entidad
    private ClassSession() { }

    /// <summary>
    /// Programa una sesión de una clase activa. Si no se indica el cupo, se usa el aforo máximo de la clase.
    /// </summary>
    public static ClassSession Schedule(
        GymClass gymClass,
        Guid instructorId,
        string instructorName,
        string room,
        DateTime startsAt,
        int? capacity)
    {
        string normalizedRoom = room?.Trim() ?? string.Empty;
        string normalizedInstructorName = instructorName?.Trim() ?? string.Empty;
        DateTime now = DateTime.UtcNow;

        if (!gymClass.IsActive)
            throw new BusinessRuleException("No se pueden programar sesiones de una clase inactiva.");

        if (instructorId == Guid.Empty || string.IsNullOrWhiteSpace(normalizedInstructorName))
            throw new BusinessRuleException("La sesión debe tener un instructor.");

        if (normalizedInstructorName.Length > INSTRUCTOR_NAME_MAX_LENGTH)
            throw new BusinessRuleException(
                $"El nombre del instructor no puede superar {INSTRUCTOR_NAME_MAX_LENGTH} caracteres.");

        if (normalizedRoom.Length < ROOM_MIN_LENGTH || normalizedRoom.Length > ROOM_MAX_LENGTH)
            throw new BusinessRuleException(
                $"El salón debe tener entre {ROOM_MIN_LENGTH} y {ROOM_MAX_LENGTH} caracteres.");

        if (startsAt <= now)
            throw new BusinessRuleException("La sesión debe programarse en una fecha futura.");

        if (startsAt > now.AddDays(MAX_DAYS_IN_ADVANCE))
            throw new BusinessRuleException(
                $"La sesión no se puede programar con más de {MAX_DAYS_IN_ADVANCE} días de anticipación.");

        int sessionCapacity = capacity ?? gymClass.MaxCapacity;
        if (sessionCapacity < GymClass.MIN_CAPACITY || sessionCapacity > gymClass.MaxCapacity)
            throw new BusinessRuleException(
                $"El cupo de la sesión debe estar entre {GymClass.MIN_CAPACITY} y {gymClass.MaxCapacity}, el aforo máximo de la clase.");

        return new ClassSession
        {
            Id = Guid.CreateVersion7(),
            GymClassId = gymClass.Id,
            InstructorId = instructorId,
            InstructorName = normalizedInstructorName,
            Room = normalizedRoom,
            TimeSlot = TimeSlot.FromDuration(startsAt, gymClass.DurationMinutes),
            Capacity = sessionCapacity,
            Status = ClassSessionStatus.Scheduled,
            CreatedAt = now
        };
    }

    public bool IsTaughtBy(Guid instructorId) => InstructorId == instructorId;

    /// <summary>
    /// Cancela una sesión que todavía no ha empezado. El motivo queda registrado.
    /// </summary>
    public void Cancel(string reason)
    {
        string normalizedReason = reason?.Trim() ?? string.Empty;
        DateTime now = DateTime.UtcNow;

        if (Status == ClassSessionStatus.Cancelled)
            throw new BusinessRuleException("La sesión ya está cancelada.");

        if (TimeSlot.StartsAt <= now)
            throw new BusinessRuleException("No se puede cancelar una sesión que ya empezó.");

        if (normalizedReason.Length < REASON_MIN_LENGTH || normalizedReason.Length > REASON_MAX_LENGTH)
            throw new BusinessRuleException(
                $"El motivo de la cancelación debe tener entre {REASON_MIN_LENGTH} y {REASON_MAX_LENGTH} caracteres.");

        Status = ClassSessionStatus.Cancelled;
        CancellationReason = normalizedReason;
        CancelledAt = now;
    }
}
