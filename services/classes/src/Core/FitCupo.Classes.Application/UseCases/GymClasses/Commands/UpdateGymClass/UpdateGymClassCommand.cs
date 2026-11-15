using FitCupo.Classes.Application.Utilities.Mediator;
using FitCupo.Classes.Domain.Entities.GymClasses;

namespace FitCupo.Classes.Application.UseCases.GymClasses.Commands.UpdateGymClass;

/// <summary>
/// Comando para actualizar los datos de una clase activa.
/// </summary>
public sealed record UpdateGymClassCommand(
    Guid Id,
    string Name,
    string Description,
    ClassCategory Category,
    int DurationMinutes,
    int MaxCapacity) : IRequest;
