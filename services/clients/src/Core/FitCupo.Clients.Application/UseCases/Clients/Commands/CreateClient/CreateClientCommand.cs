using FitCupo.Clients.Application.Utilities.Mediator;
using FitCupo.Clients.Domain.Entities.Clients.ValueObjects;

namespace FitCupo.Clients.Application.UseCases.Clients.Commands.CreateClient;

/// <summary>
/// Comando para registrar un cliente nuevo. Devuelve el Id del cliente creado.
/// </summary>
public sealed record CreateClientCommand(
    DocumentType DocumentType,
    string DocumentNumber,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    DateOnly BirthDate,
    string EmergencyContactName,
    string EmergencyContactPhone) : IRequest<Guid>;
