using FitCupo.Classes.Domain.Entities.GymClasses;

namespace FitCupo.Classes.API.Requests;

/// <summary>
/// Cuerpo de la petición para actualizar una clase. El Id viaja en la ruta.
/// </summary>
public sealed record UpdateGymClassRequest(
    string Name,
    string Description,
    ClassCategory Category,
    int DurationMinutes,
    int MaxCapacity);
