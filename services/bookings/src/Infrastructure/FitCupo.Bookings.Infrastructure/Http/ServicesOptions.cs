namespace FitCupo.Bookings.Infrastructure.Http;

/// <summary>
/// Direcciones de los microservicios que consulta Reservas (sección "Services" de appsettings).
/// </summary>
public sealed class ServicesOptions
{
    public const string SECTION_NAME = "Services";
    public const int DEFAULT_TIMEOUT_SECONDS = 5;

    public string ClassesUrl { get; init; } = null!;
    public string ClientsUrl { get; init; } = null!;

    /// <summary>
    /// Tiempo máximo de espera de cada llamada. Si el otro microservicio no responde, Reservas devuelve 503.
    /// </summary>
    public int TimeoutSeconds { get; init; } = DEFAULT_TIMEOUT_SECONDS;

    public void EnsureIsValid()
    {
        if (!Uri.TryCreate(ClassesUrl, UriKind.Absolute, out _))
            throw new InvalidOperationException("Services:ClassesUrl debe ser una URL válida.");

        if (!Uri.TryCreate(ClientsUrl, UriKind.Absolute, out _))
            throw new InvalidOperationException("Services:ClientsUrl debe ser una URL válida.");

        if (TimeoutSeconds < 1)
            throw new InvalidOperationException("Services:TimeoutSeconds debe ser mayor que cero.");
    }
}
