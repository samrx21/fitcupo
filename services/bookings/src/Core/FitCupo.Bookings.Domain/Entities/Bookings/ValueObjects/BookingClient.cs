using FitCupo.Bookings.Domain.Exceptions;

namespace FitCupo.Bookings.Domain.Entities.Bookings.ValueObjects;

/// <summary>
/// Cliente que reserva, tal como lo informa el microservicio de Clientes.
/// </summary>
public sealed record BookingClient
{
    public Guid ClientId { get; }
    public string FullName { get; }
    public string Email { get; }
    public bool IsActive { get; }

    public BookingClient(Guid clientId, string fullName, string email, bool isActive)
    {
        if (clientId == Guid.Empty)
            throw new BusinessRuleException("El cliente es obligatorio.");

        if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email))
            throw new BusinessRuleException("El cliente debe tener nombre y correo.");

        ClientId = clientId;
        FullName = fullName.Trim();
        // El correo se guarda en minúsculas para buscar las reservas del afiliado con el correo de su token
        Email = email.Trim().ToLowerInvariant();
        IsActive = isActive;
    }
}
