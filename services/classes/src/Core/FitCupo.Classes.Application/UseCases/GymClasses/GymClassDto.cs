using FitCupo.Classes.Domain.Entities.GymClasses;

namespace FitCupo.Classes.Application.UseCases.GymClasses;

/// <summary>
/// Datos de una clase del catálogo.
/// </summary>
public sealed class GymClassDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public ClassCategory Category { get; init; }
    public int DurationMinutes { get; init; }
    public int MaxCapacity { get; init; }
    public GymClassStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
