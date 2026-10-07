using FitCupo.Clients.API.Requests;
using FitCupo.Clients.Application.UseCases.Clients.Commands.ActivateClient;
using FitCupo.Clients.Application.UseCases.Clients.Commands.CreateClient;
using FitCupo.Clients.Application.UseCases.Clients.Commands.DeactivateClient;
using FitCupo.Clients.Application.UseCases.Clients.Commands.UpdateClient;
using FitCupo.Clients.Application.UseCases.Clients.Queries.GetClientById;
using FitCupo.Clients.Application.UseCases.Clients.Queries.GetClientList;
using FitCupo.Clients.Application.Utilities.Mediator;
using FitCupo.Clients.Application.Utilities.Pagination;
using FitCupo.Clients.Domain.Entities.Clients;
using Microsoft.AspNetCore.Mvc;

namespace FitCupo.Clients.API.Controllers;

/// <summary>
/// Endpoints del microservicio de clientes. El controller solo traduce HTTP a
/// commands y queries; el mediador se encarga de ejecutar el caso de uso.
/// </summary>
[ApiController]
[Route("api/clients")]
[Produces("application/json")]
public sealed class ClientsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Lista los clientes de forma paginada, con búsqueda y filtro por estado.
    /// </summary>
    /// <param name="pageNumber">Número de página (mínimo 1).</param>
    /// <param name="pageSize">Clientes por página (máximo 50).</param>
    /// <param name="search">Texto a buscar en nombre, apellido, documento o correo.</param>
    /// <param name="status">Filtra por estado: Active o Inactive.</param>
    /// <param name="cancellationToken">Token de cancelación de la petición.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginationResponse<ClientListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetList(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = PaginationRequest.DEFAULT_PAGE_SIZE,
        [FromQuery] string? search = null,
        [FromQuery] ClientStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        GetClientListQuery query = new()
        {
            Pagination = new PaginationRequest(pageNumber, pageSize),
            Search = search,
            Status = status
        };

        PaginationResponse<ClientListItemDto> result = await mediator.Send(query, cancellationToken);

        return StatusCode(StatusCodes.Status200OK, result);
    }

    /// <summary>
    /// Consulta el detalle de un cliente.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ClientDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        ClientDetailDto client = await mediator.Send(new GetClientByIdQuery(id), cancellationToken);

        return StatusCode(StatusCodes.Status200OK, client);
    }

    /// <summary>
    /// Registra un cliente nuevo.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateClientCommand command, CancellationToken cancellationToken)
    {
        Guid id = await mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>
    /// Actualiza los datos de un cliente activo.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        Guid id, [FromBody] UpdateClientRequest request, CancellationToken cancellationToken)
    {
        UpdateClientCommand command = new(
            id,
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.BirthDate,
            request.EmergencyContactName,
            request.EmergencyContactPhone);

        await mediator.Send(command, cancellationToken);

        return StatusCode(StatusCodes.Status204NoContent);
    }

    /// <summary>
    /// Desactiva un cliente. Los clientes no se eliminan para conservar su historial.
    /// </summary>
    [HttpPatch("{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeactivateClientCommand(id), cancellationToken);

        return StatusCode(StatusCodes.Status204NoContent);
    }

    /// <summary>
    /// Reactiva un cliente inactivo.
    /// </summary>
    [HttpPatch("{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new ActivateClientCommand(id), cancellationToken);

        return StatusCode(StatusCodes.Status204NoContent);
    }
}
