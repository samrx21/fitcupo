using FitCupo.Classes.API.Requests;
using FitCupo.Classes.API.Security;
using FitCupo.Classes.Application.UseCases.ClassSessions;
using FitCupo.Classes.Application.UseCases.ClassSessions.Commands.CancelClassSession;
using FitCupo.Classes.Application.UseCases.ClassSessions.Commands.ScheduleClassSession;
using FitCupo.Classes.Application.UseCases.ClassSessions.Queries.GetClassSchedule;
using FitCupo.Classes.Application.UseCases.ClassSessions.Queries.GetClassSessionById;
using FitCupo.Classes.Application.Utilities.Mediator;
using FitCupo.Classes.Application.Utilities.Pagination;
using FitCupo.Classes.Domain.Entities.ClassSessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitCupo.Classes.API.Controllers;

/// <summary>
/// Horario de clases. Todos los usuarios autenticados lo consultan; los instructores
/// programan sus sesiones y las cancelan junto con el administrador.
/// </summary>
[ApiController]
[Route("api/sessions")]
[Produces("application/json")]
[Authorize]
public sealed class ClassSessionsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Consulta el horario. Sin fechas, devuelve las sesiones de los próximos 7 días.
    /// </summary>
    /// <param name="from">Fecha inicial, por ejemplo 2026-11-20T00:00:00-05:00.</param>
    /// <param name="to">Fecha final.</param>
    /// <param name="gymClassId">Filtra por clase.</param>
    /// <param name="instructorId">Filtra por instructor (Id de usuario de Identity).</param>
    /// <param name="status">Filtra por estado: Scheduled o Cancelled.</param>
    /// <param name="pageNumber">Número de página (mínimo 1).</param>
    /// <param name="pageSize">Sesiones por página (máximo 50).</param>
    /// <param name="cancellationToken">Token de cancelación de la petición.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginationResponse<ClassSessionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSchedule(
        [FromQuery] DateTimeOffset? from = null,
        [FromQuery] DateTimeOffset? to = null,
        [FromQuery] Guid? gymClassId = null,
        [FromQuery] Guid? instructorId = null,
        [FromQuery] ClassSessionStatus? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = PaginationRequest.DEFAULT_PAGE_SIZE,
        CancellationToken cancellationToken = default)
    {
        GetClassScheduleQuery query = new()
        {
            Pagination = new PaginationRequest(pageNumber, pageSize),
            From = from,
            To = to,
            GymClassId = gymClassId,
            InstructorId = instructorId,
            Status = status
        };

        PaginationResponse<ClassSessionDto> result = await mediator.Send(query, cancellationToken);

        return StatusCode(StatusCodes.Status200OK, result);
    }

    /// <summary>
    /// Consulta el detalle de una sesión.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ClassSessionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        ClassSessionDto session = await mediator.Send(new GetClassSessionByIdQuery(id), cancellationToken);

        return StatusCode(StatusCodes.Status200OK, session);
    }

    /// <summary>
    /// Programa una sesión. Solo instructores: la sesión queda a nombre del usuario del token.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = Roles.INSTRUCTOR)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Schedule(
        [FromBody] ScheduleClassSessionRequest request, CancellationToken cancellationToken)
    {
        ScheduleClassSessionCommand command = new(
            request.GymClassId,
            User.GetUserId(),
            User.GetFullName(),
            request.Room,
            request.StartsAt,
            request.Capacity);

        Guid id = await mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>
    /// Cancela una sesión. El administrador puede cancelar cualquiera; el instructor, solo las suyas.
    /// </summary>
    [HttpPatch("{id:guid}/cancel")]
    [Authorize(Roles = $"{Roles.ADMIN},{Roles.INSTRUCTOR}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(
        Guid id, [FromBody] CancelClassSessionRequest request, CancellationToken cancellationToken)
    {
        CancelClassSessionCommand command = new(
            id,
            request.Reason,
            User.GetUserId(),
            CanCancelAnySession: User.IsInRole(Roles.ADMIN));

        await mediator.Send(command, cancellationToken);

        return StatusCode(StatusCodes.Status204NoContent);
    }
}
