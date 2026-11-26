namespace FitCupo.Bookings.Application.UseCases.Bookings.Queries.GetAvailableSchedule;

/// <summary>
/// Sesión del horario (de Clases) con sus cupos disponibles (de Reservas).
/// Si quien consulta es un afiliado, indica además si ya la reservó.
/// </summary>
public sealed class AvailableSessionDto
{
    public Guid SessionId { get; init; }
    public Guid GymClassId { get; init; }
    public string GymClassName { get; init; } = null!;
    public string Category { get; init; } = null!;
    public Guid InstructorId { get; init; }
    public string InstructorName { get; init; } = null!;
    public string Room { get; init; } = null!;
    public DateTime StartsAt { get; init; }
    public DateTime EndsAt { get; init; }
    public string Status { get; init; } = null!;
    public string? CancellationReason { get; init; }
    public int Capacity { get; init; }
    public int ReservedSpots { get; init; }
    public int AvailableSpots { get; init; }

    /// <summary>
    /// Id de la reserva del afiliado en esta sesión, o null si no la ha reservado.
    /// </summary>
    public Guid? MyBookingId { get; init; }
}
