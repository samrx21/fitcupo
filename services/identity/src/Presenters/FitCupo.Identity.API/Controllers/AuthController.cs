using FitCupo.Identity.API.Security;
using FitCupo.Identity.Application.UseCases.Auth.Commands.Login;
using FitCupo.Identity.Application.UseCases.Users;
using FitCupo.Identity.Application.UseCases.Users.Queries.GetUserById;
using FitCupo.Identity.Application.Utilities.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitCupo.Identity.API.Controllers;

/// <summary>
/// Autenticación de FitCupo: inicia sesión con correo y contraseña y emite el JWT
/// que los demás microservicios validan.
/// </summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Inicia sesión. Si las credenciales son válidas devuelve un JWT firmado con los roles del usuario.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        LoginResponseDto response = await mediator.Send(command, cancellationToken);

        return StatusCode(StatusCodes.Status200OK, response);
    }

    /// <summary>
    /// Devuelve el usuario dueño del token. Sirve para comprobar que el token es válido.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        UserDto user = await mediator.Send(new GetUserByIdQuery(User.GetUserId()), cancellationToken);

        return StatusCode(StatusCodes.Status200OK, user);
    }
}
