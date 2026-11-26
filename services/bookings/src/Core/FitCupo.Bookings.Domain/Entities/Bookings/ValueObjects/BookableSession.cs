using FitCupo.Bookings.Domain.Exceptions;

namespace FitCupo.Bookings.Domain.Entities.Bookings.ValueObjects;

/// <summary>
/// Datos de la sesión que se quiere reservar, tal como los informa el microservicio de Clases
/// en el momento de la reserva. La reserva guarda una copia para mostrar su historial
/// sin volver a consultar a Clases.
/// </summary>
public sealed record BookableSession
{
    public Guid SessionId { get; }
    public string GymClassName { get; }
    public string Room { get; }
    public Guid InstructorId { get; }
    public string InstructorName { get; }
    public DateTime StartsAt { get; }
    public DateTime EndsAt { get; }
    public int Capacity { get; }
    public bool IsScheduled { get; }

    public BookableSession(
        Guid sessionId,
        string gymClassName,
        string room,
        Guid instructorId,
        string instructorName,
        DateTime startsAt,
        DateTime endsAt,
        int capacity,
        bool isScheduled)
    {
        if (sessionId == Guid.Empty)
            throw new BusinessRuleException("La sesión es obligatoria.");

        if (string.IsNullOrWhiteSpace(gymClassName) || string.IsNullOrWhiteSpace(room)
            || string.IsNullOrWhiteSpace(instructorName))
            throw new BusinessRuleException("La sesión debe tener clase, salón e instructor.");

        if (startsAt.Kind != DateTimeKind.Utc || endsAt.Kind != DateTimeKind.Utc)
            throw new BusinessRuleException("Las fechas de la sesión deben estar en UTC.");

        if (endsAt <= startsAt)
            throw new BusinessRuleException("La sesión debe terminar después de empezar.");

        if (capacity < 1)
            throw new BusinessRuleException("La sesión debe tener al menos un cupo.");

        SessionId = sessionId;
        GymClassName = gymClassName.Trim();
        Room = room.Trim();
        InstructorId = instructorId;
        InstructorName = instructorName.Trim();
        StartsAt = startsAt;
        EndsAt = endsAt;
        Capacity = capacity;
        IsScheduled = isScheduled;
    }
}
