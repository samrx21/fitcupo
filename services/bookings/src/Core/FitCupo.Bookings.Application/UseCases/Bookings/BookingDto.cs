using FitCupo.Bookings.Domain.Entities.Bookings;

namespace FitCupo.Bookings.Application.UseCases.Bookings;

/// <summary>
/// Reserva con los datos de su sesión y de su cliente. Las fechas están en UTC.
/// </summary>
public sealed class BookingDto
{
    public Guid Id { get; init; }
    public Guid SessionId { get; init; }
    public string GymClassName { get; init; } = null!;
    public string Room { get; init; } = null!;
    public Guid InstructorId { get; init; }
    public string InstructorName { get; init; } = null!;
    public DateTime StartsAt { get; init; }
    public DateTime EndsAt { get; init; }
    public Guid ClientId { get; init; }
    public string ClientName { get; init; } = null!;
    public string ClientEmail { get; init; } = null!;
    public BookingStatus Status { get; init; }
    public DateTime BookedAt { get; init; }
    public DateTime? CancelledAt { get; init; }
}
