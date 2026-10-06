using FitCupo.Clients.Application.Contracts.Persistence;
using FitCupo.Clients.Application.Contracts.Repositories;
using FitCupo.Clients.Application.Exceptions;
using FitCupo.Clients.Application.Utilities.Mediator;
using FitCupo.Clients.Domain.Common.ValueObjects;
using FitCupo.Clients.Domain.Entities.Clients;
using FitCupo.Clients.Domain.Entities.Clients.ValueObjects;

namespace FitCupo.Clients.Application.UseCases.Clients.Commands.UpdateClient;

public sealed class UpdateClientUseCase(IClientRepository clientRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateClientCommand>
{
    public async Task Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        Client client = await clientRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"No existe un cliente con el Id {request.Id}.");

        Email email = new(request.Email);
        PhoneNumber phone = new(request.Phone);
        EmergencyContact emergencyContact = new(
            request.EmergencyContactName, new PhoneNumber(request.EmergencyContactPhone));

        if (await clientRepository.ExistsByEmailAsync(email.Value, client.Id, cancellationToken))
            throw new ConflictException($"Ya existe otro cliente registrado con el correo {email.Value}.");

        client.UpdatePersonalInfo(request.FirstName, request.LastName, request.BirthDate);
        client.UpdateContactInfo(email, phone, emergencyContact);

        await clientRepository.UpdateAsync(client, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
