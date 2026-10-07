using FitCupo.Clients.Application.Utilities.Mediator;

namespace FitCupo.Clients.Application.UseCases.Clients.Commands.DeactivateClient;

/// <summary>
/// Comando para desactivar un cliente. Los clientes no se eliminan:
/// se desactivan para conservar su historial.
/// </summary>
public sealed record DeactivateClientCommand(Guid Id) : IRequest;
