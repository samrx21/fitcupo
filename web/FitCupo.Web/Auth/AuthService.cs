using FitCupo.Web.Models;
using FitCupo.Web.Services;

namespace FitCupo.Web.Auth;

/// <summary>
/// Inicio y cierre de sesión. El login va al Gateway, que lo envía a Identity (POST /api/auth/login).
/// </summary>
public sealed class AuthService(ApiClient api, TokenStorage tokenStorage, JwtAuthenticationStateProvider authenticationState)
{
    public async Task LoginAsync(string email, string password)
    {
        LoginResponse response = await api.PostAsync<LoginResponse>("api/auth/login", new { email, password });

        await tokenStorage.SetAsync(response.AccessToken);
        authenticationState.NotifyChanged();
    }

    public async Task LogoutAsync()
    {
        await tokenStorage.ClearAsync();
        authenticationState.NotifyChanged();
    }
}
