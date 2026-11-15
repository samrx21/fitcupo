using FitCupo.Classes.Domain.Entities.GymClasses;

namespace FitCupo.Classes.Application.UseCases.GymClasses;

/// <summary>
/// Transformaciones de la entidad GymClass hacia el DTO que exponen los casos de uso.
/// </summary>
internal static class GymClassMapperExtensions
{
    public static GymClassDto ToDto(this GymClass gymClass) =>
        new()
        {
            Id = gymClass.Id,
            Name = gymClass.Name,
            Description = gymClass.Description,
            Category = gymClass.Category,
            DurationMinutes = gymClass.DurationMinutes,
            MaxCapacity = gymClass.MaxCapacity,
            Status = gymClass.Status,
            CreatedAt = gymClass.CreatedAt,
            UpdatedAt = gymClass.UpdatedAt
        };
}
