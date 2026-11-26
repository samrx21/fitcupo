using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace FitCupo.Bookings.Infrastructure.Http;

/// <summary>
/// Escribe en la consola cada llamada que Reservas hace a otro microservicio, con su resultado
/// y su duración. Sirve para evidenciar la comunicación síncrona entre microservicios.
/// </summary>
public sealed class OutgoingRequestLoggingHandler(ILogger<OutgoingRequestLoggingHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        try
        {
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            logger.LogInformation(
                "Reservas → {Method} {Url} respondió {StatusCode} en {Elapsed} ms",
                request.Method, request.RequestUri, (int)response.StatusCode, stopwatch.ElapsedMilliseconds);

            return response;
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
        {
            logger.LogWarning(
                "Reservas → {Method} {Url} falló después de {Elapsed} ms: {Error}",
                request.Method, request.RequestUri, stopwatch.ElapsedMilliseconds, exception.Message);
            throw;
        }
    }
}
