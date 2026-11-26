using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace FitCupo.Bookings.Infrastructure.Http;

/// <summary>
/// Copia el JWT de la petición que recibió Reservas en la llamada a Clases o Clientes.
/// Así cada microservicio valida el token por su cuenta y aplica sus propios permisos:
/// por ejemplo, GET /api/clients/me devuelve el perfil del afiliado dueño del token.
/// </summary>
public sealed class ForwardAuthorizationHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string? authorization = httpContextAccessor.HttpContext?.Request.Headers[HeaderNames.Authorization];

        if (!string.IsNullOrWhiteSpace(authorization))
            request.Headers.TryAddWithoutValidation(HeaderNames.Authorization, authorization);

        return base.SendAsync(request, cancellationToken);
    }
}
