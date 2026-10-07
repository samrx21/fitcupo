using FitCupo.Clients.Application.Utilities.Mediator;

namespace FitCupo.Clients.Application.UseCases.Clients.Queries.GetClientById;

/// <summary>
/// Query para consultar el detalle de un cliente por su Id.
/// </summary>
public sealed record GetClientByIdQuery(Guid Id) : IRequest<ClientDetailDto>;
