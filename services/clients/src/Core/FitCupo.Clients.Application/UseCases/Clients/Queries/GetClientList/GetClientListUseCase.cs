using FitCupo.Clients.Application.Contracts.Repositories;
using FitCupo.Clients.Application.Utilities.Mediator;
using FitCupo.Clients.Application.Utilities.Pagination;
using FitCupo.Clients.Domain.Entities.Clients;

namespace FitCupo.Clients.Application.UseCases.Clients.Queries.GetClientList;

public sealed class GetClientListUseCase(IClientRepository clientRepository)
    : IRequestHandler<GetClientListQuery, PaginationResponse<ClientListItemDto>>
{
    public async Task<PaginationResponse<ClientListItemDto>> Handle(
        GetClientListQuery request, CancellationToken cancellationToken)
    {
        PaginationResponse<Client> page = await clientRepository.GetPagedListAsync(
            request.Pagination, request.Search, request.Status, cancellationToken);

        List<ClientListItemDto> items = page.Items.Select(client => client.ToListItemDto()).ToList();

        return PaginationResponse<ClientListItemDto>.Create(items, page.TotalCount, request.Pagination);
    }
}
