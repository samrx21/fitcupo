using FitCupo.Identity.Application.Constants;
using FitCupo.Identity.Application.UseCases.Users;
using FitCupo.Identity.Application.UseCases.Users.Queries.GetUserList;
using FitCupo.Identity.Application.Utilities.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitCupo.Identity.API.Controllers;

/// <summary>
/// Consulta de los usuarios creados por los seeders. Solo lectura: en esta entrega
/// no se administran usuarios ni roles.
/// </summary>
[ApiController]
[Route("api/users")]
[Produces("application/json")]
[Authorize(Roles = Roles.ADMIN)]
public sealed class UsersController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Lista los usuarios con sus roles. Solo para administradores.
    /// </summary>
    /// <param name="role">Filtra por rol: Admin, Receptionist, Instructor o Member.</param>
    /// <param name="cancellationToken">Token de cancelación de la petición.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetList([FromQuery] string? role, CancellationToken cancellationToken)
    {
        IReadOnlyList<UserDto> users = await mediator.Send(new GetUserListQuery(role), cancellationToken);

        return StatusCode(StatusCodes.Status200OK, users);
    }
}
