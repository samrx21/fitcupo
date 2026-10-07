using FitCupo.Clients.Application.UseCases.Clients.Queries.GetClientById;
using FitCupo.Clients.Application.UseCases.Clients.Queries.GetClientList;
using FitCupo.Clients.Domain.Entities.Clients;

namespace FitCupo.Clients.Application.UseCases.Clients;

/// <summary>
/// Transformaciones de la entidad Client hacia los DTOs que exponen los casos de uso.
/// </summary>
internal static class ClientMapperExtensions
{
    public static ClientDetailDto ToDetailDto(this Client client) =>
        new()
        {
            Id = client.Id,
            FirstName = client.FirstName,
            LastName = client.LastName,
            FullName = client.FullName,
            DocumentType = client.Document.Type,
            DocumentNumber = client.Document.Number,
            Email = client.Email.Value,
            Phone = client.Phone.Value,
            BirthDate = client.BirthDate,
            EmergencyContactName = client.EmergencyContact.Name,
            EmergencyContactPhone = client.EmergencyContact.Phone.Value,
            Status = client.Status,
            CreatedAt = client.CreatedAt,
            UpdatedAt = client.UpdatedAt
        };

    public static ClientListItemDto ToListItemDto(this Client client) =>
        new()
        {
            Id = client.Id,
            FullName = client.FullName,
            DocumentType = client.Document.Type,
            DocumentNumber = client.Document.Number,
            Email = client.Email.Value,
            Phone = client.Phone.Value,
            Status = client.Status
        };
}
