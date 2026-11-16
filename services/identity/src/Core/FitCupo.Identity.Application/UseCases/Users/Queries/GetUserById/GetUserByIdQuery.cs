using FitCupo.Identity.Application.Utilities.Mediator;

namespace FitCupo.Identity.Application.UseCases.Users.Queries.GetUserById;

/// <summary>
/// Query para consultar un usuario y sus roles.
/// </summary>
public sealed record GetUserByIdQuery(Guid Id) : IRequest<UserDto>;
