using FitCupo.Clients.Application.Utilities.Mediator;
using FitCupo.Clients.Application.Utilities.Pagination;
using FitCupo.Clients.Domain.Entities.Clients;

namespace FitCupo.Clients.Application.UseCases.Clients.Queries.GetClientList;

/// <summary>
/// Query para listar clientes de forma paginada. Permite buscar por nombre,
/// documento o correo y filtrar por estado.
/// </summary>
public sealed record GetClientListQuery : IRequest<PaginationResponse<ClientListItemDto>>
{
    public PaginationRequest Pagination { get; init; } = PaginationRequest.Standard();
    public string? Search { get; init; }
    public ClientStatus? Status { get; init; }
}
