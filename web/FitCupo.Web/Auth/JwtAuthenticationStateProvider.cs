using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace FitCupo.Web.Auth;

/// <summary>
/// Indica a Blazor quién inició sesión leyendo los claims del JWT guardado.
/// Solo lee el token para mostrar u ocultar opciones: la validación real la hace cada microservicio.
/// </summary>
public sealed class JwtAuthenticationStateProvider(TokenStorage tokenStorage) : AuthenticationStateProvider
{
    private static readonly AuthenticationState Anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string? token = await tokenStorage.GetAsync();
        if (string.IsNullOrWhiteSpace(token))
            return Anonymous;

        List<Claim>? claims = ReadClaims(token);
        if (claims is null || IsExpired(claims))
        {
            await tokenStorage.ClearAsync();
            return Anonymous;
        }

        ClaimsIdentity identity = new(claims, authenticationType: "jwt", nameType: "name", roleType: "role");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public void NotifyChanged() => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

    /// <summary>
    /// Decodifica la parte central (payload) del JWT. Un claim con varios valores, como "role", llega como arreglo.
    /// </summary>
    private static List<Claim>? ReadClaims(string token)
    {
        string[] parts = token.Split('.');
        if (parts.Length != 3)
            return null;

        try
        {
            string payload = parts[1].Replace('-', '+').Replace('_', '/');
            payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
            string json = Encoding.UTF8.GetString(Convert.FromBase64String(payload));

            using JsonDocument document = JsonDocument.Parse(json);
            List<Claim> claims = [];
            foreach (JsonProperty property in document.RootElement.EnumerateObject())
            {
                if (property.Value.ValueKind == JsonValueKind.Array)
                    claims.AddRange(property.Value.EnumerateArray().Select(value => new Claim(property.Name, value.ToString())));
                else
                    claims.Add(new Claim(property.Name, property.Value.ToString()));
            }

            return claims;
        }
        catch (Exception exception) when (exception is FormatException or JsonException)
        {
            return null;
        }
    }

    private static bool IsExpired(List<Claim> claims)
    {
        string? exp = claims.FirstOrDefault(claim => claim.Type == "exp")?.Value;
        return !long.TryParse(exp, out long seconds)
               || DateTimeOffset.FromUnixTimeSeconds(seconds) <= DateTimeOffset.UtcNow;
    }
}
