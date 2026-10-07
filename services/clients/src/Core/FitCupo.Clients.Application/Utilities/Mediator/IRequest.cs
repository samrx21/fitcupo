namespace FitCupo.Clients.Application.Utilities.Mediator;

/// <summary>
/// Petición (command o query) que devuelve una respuesta de tipo <typeparamref name="TResponse"/>.
/// </summary>
public interface IRequest<TResponse>;

/// <summary>
/// Petición (command) que no devuelve respuesta.
/// </summary>
public interface IRequest;
