using FitCupo.Classes.Application.Utilities.Mediator;

namespace FitCupo.Classes.Application.UseCases.GymClasses.Commands.ActivateGymClass;

/// <summary>
/// Comando para volver a ofrecer una clase inactiva.
/// </summary>
public sealed record ActivateGymClassCommand(Guid Id) : IRequest;
