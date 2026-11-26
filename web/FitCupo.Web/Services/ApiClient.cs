using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FitCupo.Web.Auth;
using Microsoft.AspNetCore.Components;

namespace FitCupo.Web.Services;

/// <summary>
/// Cliente HTTP de la interfaz. Todas las peticiones van al API Gateway con el JWT en el
/// encabezado Authorization; los errores se convierten en ApiException con el mensaje del microservicio.
/// </summary>
public sealed class ApiClient(
    HttpClient httpClient,
    TokenStorage tokenStorage,
    JwtAuthenticationStateProvider authenticationState,
    NavigationManager navigation)
{
    public static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task<T> GetAsync<T>(string url) =>
        await ReadAsync<T>(await SendAsync(HttpMethod.Get, url));

    public async Task<T> PostAsync<T>(string url, object body) =>
        await ReadAsync<T>(await SendAsync(HttpMethod.Post, url, body));

    public async Task PostAsync(string url, object body) =>
        (await SendAsync(HttpMethod.Post, url, body)).Dispose();

    public async Task PatchAsync(string url, object? body = null) =>
        (await SendAsync(HttpMethod.Patch, url, body)).Dispose();

    public async Task PutAsync(string url, object body) =>
        (await SendAsync(HttpMethod.Put, url, body)).Dispose();

    private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string url, object? body = null)
    {
        using HttpRequestMessage request = new(method, url);
        if (body is not null)
            request.Content = JsonContent.Create(body, options: JsonOptions);

        string? token = await tokenStorage.GetAsync();
        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        HttpResponseMessage response;
        try
        {
            response = await httpClient.SendAsync(request);
        }
        catch (HttpRequestException)
        {
            throw new ApiException(0, "Sin conexión",
                "No se pudo conectar con el API Gateway (http://localhost:5100). Verifica que esté en ejecución.");
        }

        if (response.IsSuccessStatusCode)
            return response;

        using (response)
        {
            // El token expiró o no es válido: se cierra la sesión y se vuelve al inicio de sesión
            if (response.StatusCode == HttpStatusCode.Unauthorized && !string.IsNullOrWhiteSpace(token))
            {
                await tokenStorage.ClearAsync();
                authenticationState.NotifyChanged();
                navigation.NavigateTo("login");
            }

            throw await ToExceptionAsync(response);
        }
    }

    private static async Task<T> ReadAsync<T>(HttpResponseMessage response)
    {
        using (response)
        {
            return (await response.Content.ReadFromJsonAsync<T>(JsonOptions))!;
        }
    }

    private static async Task<ApiException> ToExceptionAsync(HttpResponseMessage response)
    {
        int status = (int)response.StatusCode;
        try
        {
            ProblemDetailsResponse? problem = await response.Content.ReadFromJsonAsync<ProblemDetailsResponse>(JsonOptions);
            if (problem is not null)
                return new ApiException(status, problem.Title ?? $"Error {status}", problem.Detail ?? problem.Title ?? $"Error {status}");
        }
        catch (Exception exception) when (exception is JsonException or NotSupportedException)
        {
            // La respuesta no trae ProblemDetails (por ejemplo, 502 del Gateway con un microservicio caído)
        }

        string detail = response.StatusCode switch
        {
            HttpStatusCode.BadGateway or HttpStatusCode.ServiceUnavailable or HttpStatusCode.GatewayTimeout =>
                "El microservicio que atiende esta petición no está disponible.",
            _ => $"La petición falló con el código {status}."
        };
        return new ApiException(status, $"Error {status}", detail);
    }

    private sealed record ProblemDetailsResponse(string? Title, string? Detail, int? Status);
}
