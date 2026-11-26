using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FitCupo.Bookings.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace FitCupo.Bookings.Infrastructure.Services;

/// <summary>
/// Base de los clientes HTTP hacia otros microservicios. Traduce las respuestas a lo que
/// entiende la capa de aplicación: 404 → null, 401/403 → ForbiddenException y
/// caída, tiempo de espera o error 5xx → ServiceUnavailableException (503).
/// </summary>
public abstract class RemoteServiceClient(HttpClient httpClient, string serviceName)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    protected async Task<T?> GetAsync<T>(string path, CancellationToken cancellationToken) where T : class
    {
        HttpResponseMessage response;
        try
        {
            response = await httpClient.GetAsync(path, cancellationToken);
        }
        catch (HttpRequestException)
        {
            throw new ServiceUnavailableException(
                $"El microservicio de {serviceName} no está disponible. Intenta de nuevo en unos minutos.");
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new ServiceUnavailableException(
                $"El microservicio de {serviceName} no respondió a tiempo. Intenta de nuevo en unos minutos.");
        }

        using (response)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
                throw new ForbiddenException(
                    await ReadDetailAsync(response, cancellationToken)
                    ?? $"El microservicio de {serviceName} rechazó la consulta.");

            if (!response.IsSuccessStatusCode)
                throw new ServiceUnavailableException(
                    $"El microservicio de {serviceName} respondió con un error ({(int)response.StatusCode}).");

            return await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
        }
    }

    /// <summary>
    /// Lee el campo "detail" del ProblemDetails que responden los otros microservicios.
    /// </summary>
    private static async Task<string?> ReadDetailAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            ProblemDetails? problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(JsonOptions, cancellationToken);
            return problem?.Detail;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
