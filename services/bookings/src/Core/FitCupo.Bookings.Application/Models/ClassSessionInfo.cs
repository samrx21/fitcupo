namespace FitCupo.Bookings.Application.Models;

/// <summary>
/// Sesión del horario tal como la responde el microservicio de Clases (GET /api/sessions).
/// Las fechas están en UTC.
/// </summary>
public sealed class ClassSessionInfo
{
    public const string SCHEDULED_STATUS = "Scheduled";

    public Guid Id { get; init; }
    public Guid GymClassId { get; init; }
    public string GymClassName { get; init; } = null!;
    public string Category { get; init; } = null!;
    public Guid InstructorId { get; init; }
    public string InstructorName { get; init; } = null!;
    public string Room { get; init; } = null!;
    public DateTime StartsAt { get; init; }
    public DateTime EndsAt { get; init; }
    public int Capacity { get; init; }
    public string Status { get; init; } = null!;
    public string? CancellationReason { get; init; }

    public bool IsScheduled => Status == SCHEDULED_STATUS;
}
