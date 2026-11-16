using FitCupo.Classes.Application.Utilities.Mediator;
using FitCupo.Classes.Domain.Entities.GymClasses;

namespace FitCupo.Classes.Application.UseCases.GymClasses.Commands.CreateGymClass;

/// <summary>
/// Comando para agregar una clase al catálogo. Devuelve el Id de la clase creada.
/// </summary>
public sealed record CreateGymClassCommand(
    string Name,
    string Description,
    ClassCategory Category,
    int DurationMinutes,
    int MaxCapacity) : IRequest<Guid>;
