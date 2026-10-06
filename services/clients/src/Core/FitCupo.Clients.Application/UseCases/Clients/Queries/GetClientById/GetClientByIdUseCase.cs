using FitCupo.Clients.Application.Contracts.Repositories;
using FitCupo.Clients.Application.Exceptions;
using FitCupo.Clients.Application.Utilities.Mediator;
using FitCupo.Clients.Domain.Entities.Clients;

namespace FitCupo.Clients.Application.UseCases.Clients.Queries.GetClientById;

public sealed class GetClientByIdUseCase(IClientRepository clientRepository)
    : IRequestHandler<GetClientByIdQuery, ClientDetailDto>
{
    public async Task<ClientDetailDto> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        Client client = await clientRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"No existe un cliente con el Id {request.Id}.");

        return client.ToDetailDto();
    }
}
