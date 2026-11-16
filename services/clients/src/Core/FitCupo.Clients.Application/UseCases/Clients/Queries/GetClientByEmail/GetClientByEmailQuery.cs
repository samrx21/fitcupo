using FitCupo.Clients.Application.UseCases.Clients.Queries.GetClientById;
using FitCupo.Clients.Application.Utilities.Mediator;

namespace FitCupo.Clients.Application.UseCases.Clients.Queries.GetClientByEmail;

/// <summary>
/// Query para consultar un cliente por su correo. La usa el afiliado para ver su propio
/// perfil: el correo viene del JWT que emitió Identity.
/// </summary>
public sealed record GetClientByEmailQuery(string Email) : IRequest<ClientDetailDto>;
