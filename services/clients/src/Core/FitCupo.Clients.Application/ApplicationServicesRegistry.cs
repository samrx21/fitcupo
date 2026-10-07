using System.Reflection;
using FitCupo.Clients.Application.Utilities.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace FitCupo.Clients.Application;

public static class ApplicationServicesRegistry
{
    /// <summary>
    /// Registra el mediador y todos los casos de uso de la capa de aplicación
    /// en el contenedor de dependencias.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IMediator, SimpleMediator>();
        services.AddUseCases(typeof(ApplicationServicesRegistry).Assembly);

        return services;
    }

    /// <summary>
    /// Busca en el ensamblado todas las clases que implementan IRequestHandler y las
    /// registra. Así, al crear un caso de uso nuevo no hay que modificar este archivo.
    /// </summary>
    private static void AddUseCases(this IServiceCollection services, Assembly assembly)
    {
        Type[] handlerInterfaces = [typeof(IRequestHandler<,>), typeof(IRequestHandler<>)];

        IEnumerable<Type> useCaseTypes = assembly.GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false });

        foreach (Type useCaseType in useCaseTypes)
        {
            IEnumerable<Type> implementedHandlers = useCaseType.GetInterfaces()
                .Where(@interface => @interface.IsGenericType
                    && handlerInterfaces.Contains(@interface.GetGenericTypeDefinition()));

            foreach (Type handlerInterface in implementedHandlers)
                services.AddScoped(handlerInterface, useCaseType);
        }
    }
}
