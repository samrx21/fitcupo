namespace FitCupo.Bookings.Application.UseCases.Bookings.Queries.GetSessionBookings;

/// <summary>
/// Lista de inscritos de una sesión: los datos de la sesión (de Clases) y sus reservas confirmadas.
/// </summary>
public sealed class SessionBookingsDto
{
    public Guid SessionId { get; init; }
    public string GymClassName { get; init; } = null!;
    public string Room { get; init; } = null!;
    public string InstructorName { get; init; } = null!;
    public DateTime StartsAt { get; init; }
    public DateTime EndsAt { get; init; }
    public string SessionStatus { get; init; } = null!;
    public int Capacity { get; init; }
    public int ReservedSpots { get; init; }
    public int AvailableSpots { get; init; }
    public required IReadOnlyList<BookingDto> Bookings { get; init; }
}
