using FitCupo.Classes.Application.Utilities.Mediator;

namespace FitCupo.Classes.Application.UseCases.GymClasses.Commands.DeactivateGymClass;

/// <summary>
/// Comando para retirar una clase del catálogo. Las clases no se eliminan
/// para conservar el historial de sus sesiones.
/// </summary>
public sealed record DeactivateGymClassCommand(Guid Id) : IRequest;
