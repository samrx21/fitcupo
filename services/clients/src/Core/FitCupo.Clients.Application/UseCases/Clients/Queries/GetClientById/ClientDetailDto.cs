using FitCupo.Clients.Domain.Entities.Clients;
using FitCupo.Clients.Domain.Entities.Clients.ValueObjects;

namespace FitCupo.Clients.Application.UseCases.Clients.Queries.GetClientById;

/// <summary>
/// Detalle completo de un cliente. Nunca se expone la entidad del dominio directamente.
/// </summary>
public sealed class ClientDetailDto
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string FullName { get; init; } = null!;
    public DocumentType DocumentType { get; init; }
    public string DocumentNumber { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Phone { get; init; } = null!;
    public DateOnly BirthDate { get; init; }
    public string EmergencyContactName { get; init; } = null!;
    public string EmergencyContactPhone { get; init; } = null!;
    public ClientStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
