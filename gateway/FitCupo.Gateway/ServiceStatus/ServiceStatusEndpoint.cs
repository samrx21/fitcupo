using System.Diagnostics;
using Yarp.ReverseProxy;
using Yarp.ReverseProxy.Model;

namespace FitCupo.Gateway.ServiceStatus;

/// <summary>
/// GET /api/status: el Gateway llama a cada microservicio configurado en YARP y responde
/// si está disponible y cuánto tardó. La interfaz web lo muestra en la página de inicio.
/// </summary>
public static class ServiceStatusEndpoint
{
    public const string HTTP_CLIENT_NAME = "service-status";
    private const string DISPLAY_NAME_KEY = "DisplayName";
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(2);

    public static IEndpointRouteBuilder MapServiceStatus(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/status", GetStatusAsync);
        return app;
    }

    private static async Task<IResult> GetStatusAsync(
        IProxyStateLookup proxyState, IHttpClientFactory httpClientFactory, CancellationToken cancellationToken)
    {
        HttpClient httpClient = httpClientFactory.CreateClient(HTTP_CLIENT_NAME);
        httpClient.Timeout = Timeout;

        IEnumerable<Task<ServiceStatusDto>> checks = proxyState.GetClusters()
            .Select(cluster => CheckAsync(httpClient, cluster, cancellationToken));

        ServiceStatusDto[] services = await Task.WhenAll(checks);

        return Results.Ok(services);
    }

    private static async Task<ServiceStatusDto> CheckAsync(
        HttpClient httpClient, ClusterState cluster, CancellationToken cancellationToken)
    {
        string name = cluster.Model.Config.Metadata?.GetValueOrDefault(DISPLAY_NAME_KEY) ?? cluster.ClusterId;
        string address = cluster.Model.Config.Destinations?.Values.FirstOrDefault()?.Address ?? string.Empty;
        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            // Cualquier respuesta HTTP (incluso 404) indica que el microservicio está escuchando
            using HttpResponseMessage response = await httpClient.GetAsync(address, cancellationToken);
            return new ServiceStatusDto(name, address, true, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
        {
            return new ServiceStatusDto(name, address, false, stopwatch.ElapsedMilliseconds);
        }
    }

    public sealed record ServiceStatusDto(string Name, string Address, bool IsAvailable, long ElapsedMilliseconds);
}
