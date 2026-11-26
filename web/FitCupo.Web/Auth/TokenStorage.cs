using Microsoft.JSInterop;

namespace FitCupo.Web.Auth;

/// <summary>
/// Guarda el JWT en el localStorage del navegador para no perder la sesión al recargar la página.
/// </summary>
public sealed class TokenStorage(IJSRuntime js)
{
    private const string KEY = "fitcupo.token";

    // Copia en memoria para no llamar a JavaScript en cada petición
    private string? _token;
    private bool _loaded;

    public async ValueTask<string?> GetAsync()
    {
        if (!_loaded)
        {
            _token = await js.InvokeAsync<string?>("localStorage.getItem", KEY);
            _loaded = true;
        }

        return _token;
    }

    public async ValueTask SetAsync(string token)
    {
        _token = token;
        _loaded = true;
        await js.InvokeVoidAsync("localStorage.setItem", KEY, token);
    }

    public async ValueTask ClearAsync()
    {
        _token = null;
        _loaded = true;
        await js.InvokeVoidAsync("localStorage.removeItem", KEY);
    }
}
