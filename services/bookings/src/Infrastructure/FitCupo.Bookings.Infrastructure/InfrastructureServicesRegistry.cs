using FitCupo.Bookings.Application.Contracts.Services;
using FitCupo.Bookings.Infrastructure.Http;
using FitCupo.Bookings.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FitCupo.Bookings.Infrastructure;

public static class InfrastructureServicesRegistry
{
    /// <summary>
    /// Registra los clientes HTTP hacia Clases y Clientes con IHttpClientFactory. Cada llamada
    /// lleva el JWT del usuario y queda registrada en la consola con su duración.
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        ServicesOptions options = configuration.GetSection(ServicesOptions.SECTION_NAME).Get<ServicesOptions>()
            ?? throw new InvalidOperationException($"La sección '{ServicesOptions.SECTION_NAME}' no está configurada.");
        options.EnsureIsValid();

        services.AddHttpContextAccessor();
        services.AddTransient<ForwardAuthorizationHandler>();
        services.AddTransient<OutgoingRequestLoggingHandler>();

        services.AddHttpClient<IClassesService, ClassesService>(client => Configure(client, options.ClassesUrl, options))
            .AddHttpMessageHandler<ForwardAuthorizationHandler>()
            .AddHttpMessageHandler<OutgoingRequestLoggingHandler>();

        services.AddHttpClient<IClientsService, ClientsService>(client => Configure(client, options.ClientsUrl, options))
            .AddHttpMessageHandler<ForwardAuthorizationHandler>()
            .AddHttpMessageHandler<OutgoingRequestLoggingHandler>();

        return services;
    }

    private static void Configure(HttpClient client, string baseUrl, ServicesOptions options)
    {
        // La barra final permite combinar la URL base con rutas relativas como "api/sessions"
        client.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
        client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
    }
}
