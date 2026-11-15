using FitCupo.Classes.Domain.Entities.ClassSessions;
using FitCupo.Classes.Domain.Entities.GymClasses;

namespace FitCupo.Classes.Application.UseCases.ClassSessions;

/// <summary>
/// Sesión del horario con los datos de su clase. Las fechas están en UTC.
/// </summary>
public sealed class ClassSessionDto
{
    public Guid Id { get; init; }
    public Guid GymClassId { get; init; }
    public string GymClassName { get; init; } = null!;
    public ClassCategory Category { get; init; }
    public Guid InstructorId { get; init; }
    public string InstructorName { get; init; } = null!;
    public string Room { get; init; } = null!;
    public DateTime StartsAt { get; init; }
    public DateTime EndsAt { get; init; }
    public int Capacity { get; init; }
    public ClassSessionStatus Status { get; init; }
    public string? CancellationReason { get; init; }
}
