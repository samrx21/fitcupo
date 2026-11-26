using FitCupo.Bookings.Application.Models;

namespace FitCupo.Bookings.Application.Contracts.Services;

/// <summary>
/// Comunicación síncrona (HTTP) con el microservicio de Clientes. Se implementa en Infraestructura.
/// </summary>
public interface IClientsService
{
    /// <summary>
    /// Perfil del afiliado que inició sesión (GET /api/clients/me con su token).
    /// Devuelve null si el afiliado todavía no está registrado como cliente.
    /// </summary>
    Task<ClientInfo?> GetMyProfileAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Cliente por su Id (GET /api/clients/{id}); lo usa recepción al reservar para un cliente.
    /// Devuelve null si no existe.
    /// </summary>
    Task<ClientInfo?> GetClientAsync(Guid clientId, CancellationToken cancellationToken = default);
}
