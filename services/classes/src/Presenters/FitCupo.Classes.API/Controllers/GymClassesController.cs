using FitCupo.Classes.API.Requests;
using FitCupo.Classes.API.Security;
using FitCupo.Classes.Application.UseCases.GymClasses;
using FitCupo.Classes.Application.UseCases.GymClasses.Commands.ActivateGymClass;
using FitCupo.Classes.Application.UseCases.GymClasses.Commands.CreateGymClass;
using FitCupo.Classes.Application.UseCases.GymClasses.Commands.DeactivateGymClass;
using FitCupo.Classes.Application.UseCases.GymClasses.Commands.UpdateGymClass;
using FitCupo.Classes.Application.UseCases.GymClasses.Queries.GetGymClassById;
using FitCupo.Classes.Application.UseCases.GymClasses.Queries.GetGymClassList;
using FitCupo.Classes.Application.Utilities.Mediator;
using FitCupo.Classes.Application.Utilities.Pagination;
using FitCupo.Classes.Domain.Entities.GymClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitCupo.Classes.API.Controllers;

/// <summary>
/// Catálogo de clases del gimnasio. Cualquier usuario autenticado puede consultarlo;
/// solo el administrador lo modifica.
/// </summary>
[ApiController]
[Route("api/classes")]
[Produces("application/json")]
[Authorize]
public sealed class GymClassesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Lista las clases de forma paginada, con búsqueda y filtros por categoría y estado.
    /// </summary>
    /// <param name="pageNumber">Número de página (mínimo 1).</param>
    /// <param name="pageSize">Clases por página (máximo 50).</param>
    /// <param name="search">Texto a buscar en el nombre o la descripción.</param>
    /// <param name="category">Filtra por categoría, por ejemplo Yoga o Cycling.</param>
    /// <param name="status">Filtra por estado: Active o Inactive.</param>
    /// <param name="cancellationToken">Token de cancelación de la petición.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginationResponse<GymClassDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetList(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = PaginationRequest.DEFAULT_PAGE_SIZE,
        [FromQuery] string? search = null,
        [FromQuery] ClassCategory? category = null,
        [FromQuery] GymClassStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        GetGymClassListQuery query = new()
        {
            Pagination = new PaginationRequest(pageNumber, pageSize),
            Search = search,
            Category = category,
            Status = status
        };

        PaginationResponse<GymClassDto> result = await mediator.Send(query, cancellationToken);

        return StatusCode(StatusCodes.Status200OK, result);
    }

    /// <summary>
    /// Consulta el detalle de una clase.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GymClassDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        GymClassDto gymClass = await mediator.Send(new GetGymClassByIdQuery(id), cancellationToken);

        return StatusCode(StatusCodes.Status200OK, gymClass);
    }

    /// <summary>
    /// Agrega una clase al catálogo. Solo administradores.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = Roles.ADMIN)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateGymClassCommand command, CancellationToken cancellationToken)
    {
        Guid id = await mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>
    /// Actualiza una clase activa. Solo administradores.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.ADMIN)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        Guid id, [FromBody] UpdateGymClassRequest request, CancellationToken cancellationToken)
    {
        UpdateGymClassCommand command = new(
            id,
            request.Name,
            request.Description,
            request.Category,
            request.DurationMinutes,
            request.MaxCapacity);

        await mediator.Send(command, cancellationToken);

        return StatusCode(StatusCodes.Status204NoContent);
    }

    /// <summary>
    /// Retira una clase del catálogo. Solo administradores.
    /// </summary>
    [HttpPatch("{id:guid}/deactivate")]
    [Authorize(Roles = Roles.ADMIN)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeactivateGymClassCommand(id), cancellationToken);

        return StatusCode(StatusCodes.Status204NoContent);
    }

    /// <summary>
    /// Vuelve a ofrecer una clase inactiva. Solo administradores.
    /// </summary>
    [HttpPatch("{id:guid}/activate")]
    [Authorize(Roles = Roles.ADMIN)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new ActivateGymClassCommand(id), cancellationToken);

        return StatusCode(StatusCodes.Status204NoContent);
    }
}
