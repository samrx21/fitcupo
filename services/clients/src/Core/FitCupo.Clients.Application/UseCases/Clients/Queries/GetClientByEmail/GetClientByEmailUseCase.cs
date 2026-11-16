using FitCupo.Clients.Application.Contracts.Repositories;
using FitCupo.Clients.Application.Exceptions;
using FitCupo.Clients.Application.UseCases.Clients.Queries.GetClientById;
using FitCupo.Clients.Application.Utilities.Mediator;
using FitCupo.Clients.Domain.Common.ValueObjects;
using FitCupo.Clients.Domain.Entities.Clients;

namespace FitCupo.Clients.Application.UseCases.Clients.Queries.GetClientByEmail;

public sealed class GetClientByEmailUseCase(IClientRepository clientRepository)
    : IRequestHandler<GetClientByEmailQuery, ClientDetailDto>
{
    public async Task<ClientDetailDto> Handle(GetClientByEmailQuery request, CancellationToken cancellationToken)
    {
        // El value object normaliza el correo igual que cuando se registró el cliente
        Email email = new(request.Email);

        Client client = await clientRepository.GetByEmailAsync(email.Value, cancellationToken)
            ?? throw new NotFoundException(
                $"No hay un cliente registrado con el correo {email.Value}. Acércate a recepción para registrarte.");

        return client.ToDetailDto();
    }
}
