namespace FitCupo.Clients.Application.Utilities.Mediator;

/// <summary>
/// Caso de uso que atiende una petición y devuelve una respuesta.
/// </summary>
public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// Caso de uso que atiende una petición sin respuesta.
/// </summary>
public interface IRequestHandler<in TRequest>
    where TRequest : IRequest
{
    Task Handle(TRequest request, CancellationToken cancellationToken);
}
