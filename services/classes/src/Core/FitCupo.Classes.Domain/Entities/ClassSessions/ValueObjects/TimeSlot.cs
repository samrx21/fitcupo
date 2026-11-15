using FitCupo.Classes.Domain.Exceptions;

namespace FitCupo.Classes.Domain.Entities.ClassSessions.ValueObjects;

/// <summary>
/// Franja horaria de una sesión, en UTC. Se valida que termine después de empezar.
/// </summary>
public sealed record TimeSlot
{
    public DateTime StartsAt { get; private set; }
    public DateTime EndsAt { get; private set; }

    // Requerido por EF Core
    private TimeSlot() { }

    public TimeSlot(DateTime startsAt, DateTime endsAt)
    {
        if (startsAt.Kind != DateTimeKind.Utc || endsAt.Kind != DateTimeKind.Utc)
            throw new BusinessRuleException("Las fechas de la sesión deben estar en UTC.");

        if (endsAt <= startsAt)
            throw new BusinessRuleException("La sesión debe terminar después de empezar.");

        StartsAt = startsAt;
        EndsAt = endsAt;
    }

    public static TimeSlot FromDuration(DateTime startsAt, int durationMinutes) =>
        new(startsAt, startsAt.AddMinutes(durationMinutes));

    /// <summary>
    /// Dos franjas se cruzan si una empieza antes de que la otra termine.
    /// </summary>
    public bool Overlaps(TimeSlot other) => StartsAt < other.EndsAt && other.StartsAt < EndsAt;
}
