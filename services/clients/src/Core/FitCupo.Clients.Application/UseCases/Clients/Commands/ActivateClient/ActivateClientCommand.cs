using FitCupo.Clients.Application.Utilities.Mediator;

namespace FitCupo.Clients.Application.UseCases.Clients.Commands.ActivateClient;

/// <summary>
/// Comando para reactivar un cliente que estaba inactivo.
/// </summary>
public sealed record ActivateClientCommand(Guid Id) : IRequest;
