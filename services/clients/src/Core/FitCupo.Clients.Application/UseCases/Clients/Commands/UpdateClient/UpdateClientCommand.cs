using FitCupo.Clients.Application.Utilities.Mediator;

namespace FitCupo.Clients.Application.UseCases.Clients.Commands.UpdateClient;

/// <summary>
/// Comando para actualizar los datos de un cliente. El documento no se puede
/// cambiar porque identifica al cliente en el gimnasio.
/// </summary>
public sealed record UpdateClientCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    DateOnly BirthDate,
    string EmergencyContactName,
    string EmergencyContactPhone) : IRequest;
