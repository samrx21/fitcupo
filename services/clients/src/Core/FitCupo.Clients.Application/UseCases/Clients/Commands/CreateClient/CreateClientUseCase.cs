using FitCupo.Clients.Application.Contracts.Persistence;
using FitCupo.Clients.Application.Contracts.Repositories;
using FitCupo.Clients.Application.Exceptions;
using FitCupo.Clients.Application.Utilities.Mediator;
using FitCupo.Clients.Domain.Common.ValueObjects;
using FitCupo.Clients.Domain.Entities.Clients;
using FitCupo.Clients.Domain.Entities.Clients.ValueObjects;

namespace FitCupo.Clients.Application.UseCases.Clients.Commands.CreateClient;

public sealed class CreateClientUseCase(IClientRepository clientRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateClientCommand, Guid>
{
    public async Task<Guid> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        // Los value objects validan su propio formato al construirse
        Document document = new(request.DocumentType, request.DocumentNumber);
        Email email = new(request.Email);
        PhoneNumber phone = new(request.Phone);
        EmergencyContact emergencyContact = new(
            request.EmergencyContactName, new PhoneNumber(request.EmergencyContactPhone));

        // La unicidad depende de los datos guardados, por eso se valida aquí y no en el dominio
        if (await clientRepository.ExistsByDocumentAsync(document.Type, document.Number, cancellationToken))
            throw new ConflictException($"Ya existe un cliente registrado con el documento {document.Number}.");

        if (await clientRepository.ExistsByEmailAsync(email.Value, cancellationToken: cancellationToken))
            throw new ConflictException($"Ya existe un cliente registrado con el correo {email.Value}.");

        Client client = new(
            request.FirstName,
            request.LastName,
            document,
            email,
            phone,
            request.BirthDate,
            emergencyContact);

        await clientRepository.AddAsync(client, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return client.Id;
    }
}
