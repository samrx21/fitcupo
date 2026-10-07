using System.Reflection;
using FitCupo.Clients.Application.Exceptions;

namespace FitCupo.Clients.Application.Utilities.Mediator;

/// <summary>
/// Implementación del patrón mediador. Por medio de reflexión busca en el
/// contenedor de dependencias el caso de uso (handler) registrado para el
/// tipo de petición recibido y ejecuta su método Handle.
/// </summary>
public sealed class SimpleMediator(IServiceProvider serviceProvider) : IMediator
{
    private const string HANDLE_METHOD = nameof(IRequestHandler<IRequest>.Handle);

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        Type handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        object handler = ResolveHandler(handlerType, request);

        MethodInfo handleMethod = handlerType.GetMethod(HANDLE_METHOD)!;
        Task<TResponse> task = (Task<TResponse>)InvokeHandle(handleMethod, handler, request, cancellationToken);

        return await task;
    }

    public async Task Send(IRequest request, CancellationToken cancellationToken = default)
    {
        Type handlerType = typeof(IRequestHandler<>).MakeGenericType(request.GetType());
        object handler = ResolveHandler(handlerType, request);

        MethodInfo handleMethod = handlerType.GetMethod(HANDLE_METHOD)!;
        Task task = (Task)InvokeHandle(handleMethod, handler, request, cancellationToken);

        await task;
    }

    private object ResolveHandler(Type handlerType, object request) =>
        serviceProvider.GetService(handlerType)
        ?? throw new MediatorException(
            $"No se encontró un caso de uso registrado para la petición {request.GetType().Name}.");

    private static object InvokeHandle(MethodInfo handleMethod, object handler, object request, CancellationToken cancellationToken)
    {
        try
        {
            return handleMethod.Invoke(handler, [request, cancellationToken])!;
        }
        catch (TargetInvocationException exception) when (exception.InnerException is not null)
        {
            // Se relanza la excepción original del caso de uso para no envolverla en la de reflexión
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
            throw;
        }
    }
}
