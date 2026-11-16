using FitCupo.Identity.Application.Utilities.Mediator;

namespace FitCupo.Identity.Application.UseCases.Users.Queries.GetUserList;

/// <summary>
/// Query para listar los usuarios con sus roles. Permite filtrar por rol.
/// </summary>
public sealed record GetUserListQuery(string? Role) : IRequest<IReadOnlyList<UserDto>>;
