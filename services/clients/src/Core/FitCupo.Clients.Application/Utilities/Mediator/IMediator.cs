namespace FitCupo.Clients.Application.Utilities.Mediator;

/// <summary>
/// Punto central para ejecutar casos de uso. Quien envía la petición
/// (por ejemplo, un controller) no conoce qué caso de uso la atiende.
/// </summary>
public interface IMediator
{
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

    Task Send(IRequest request, CancellationToken cancellationToken = default);
}
