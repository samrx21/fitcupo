using FitCupo.Bookings.Application.Contracts.Services;
using FitCupo.Bookings.Application.Models;

namespace FitCupo.Bookings.Infrastructure.Services;

/// <summary>
/// Cliente HTTP del microservicio de Clientes.
/// </summary>
public sealed class ClientsService(HttpClient httpClient) : RemoteServiceClient(httpClient, "Clientes"), IClientsService
{
    public Task<ClientInfo?> GetMyProfileAsync(CancellationToken cancellationToken = default) =>
        GetAsync<ClientInfo>("api/clients/me", cancellationToken);

    public Task<ClientInfo?> GetClientAsync(Guid clientId, CancellationToken cancellationToken = default) =>
        GetAsync<ClientInfo>($"api/clients/{clientId}", cancellationToken);
}
