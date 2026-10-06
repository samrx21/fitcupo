using FitCupo.Clients.Application.Contracts.Persistence;
using FitCupo.Clients.Application.Contracts.Repositories;
using FitCupo.Clients.Application.Exceptions;
using FitCupo.Clients.Application.Utilities.Mediator;
using FitCupo.Clients.Domain.Entities.Clients;

namespace FitCupo.Clients.Application.UseCases.Clients.Commands.DeactivateClient;

public sealed class DeactivateClientUseCase(IClientRepository clientRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeactivateClientCommand>
{
    public async Task Handle(DeactivateClientCommand request, CancellationToken cancellationToken)
    {
        Client client = await clientRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"No existe un cliente con el Id {request.Id}.");

        client.Deactivate();

        await clientRepository.UpdateAsync(client, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
