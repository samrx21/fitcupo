using FitCupo.Clients.Domain.Entities.Clients;
using FitCupo.Clients.Domain.Entities.Clients.ValueObjects;

namespace FitCupo.Clients.Application.UseCases.Clients.Queries.GetClientList;

/// <summary>
/// Datos resumidos de un cliente para mostrar en un listado.
/// </summary>
public sealed class ClientListItemDto
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = null!;
    public DocumentType DocumentType { get; init; }
    public string DocumentNumber { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Phone { get; init; } = null!;
    public ClientStatus Status { get; init; }
}
