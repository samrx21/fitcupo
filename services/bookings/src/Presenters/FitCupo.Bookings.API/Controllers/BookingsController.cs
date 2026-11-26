using FitCupo.Bookings.API.Requests;
using FitCupo.Bookings.API.Security;
using FitCupo.Bookings.Application.Exceptions;
using FitCupo.Bookings.Application.UseCases.Bookings;
using FitCupo.Bookings.Application.UseCases.Bookings.Commands.CancelBooking;
using FitCupo.Bookings.Application.UseCases.Bookings.Commands.CreateBooking;
using FitCupo.Bookings.Application.UseCases.Bookings.Queries.GetAvailableSchedule;
using FitCupo.Bookings.Application.UseCases.Bookings.Queries.GetBookingById;
using FitCupo.Bookings.Application.UseCases.Bookings.Queries.GetBookingList;
using FitCupo.Bookings.Application.UseCases.Bookings.Queries.GetMyBookings;
using FitCupo.Bookings.Application.UseCases.Bookings.Queries.GetSessionBookings;
using FitCupo.Bookings.Application.Utilities.Mediator;
using FitCupo.Bookings.Application.Utilities.Pagination;
using FitCupo.Bookings.Domain.Entities.Bookings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitCupo.Bookings.API.Controllers;

/// <summary>
/// Reservas de cupos en las sesiones. Para validar cada reserva, Reservas consulta de forma
/// síncrona a Clientes (quién reserva y si está activo) y a Clases (la sesión y su cupo).
/// </summary>
[ApiController]
[Route("api/bookings")]
[Produces("application/json")]
[Authorize]
public sealed class BookingsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Horario con cupos disponibles: une las sesiones de Clases con las reservas de este microservicio.
    /// Para un afiliado, indica además qué sesiones ya reservó. Sin fechas, devuelve los próximos 7 días.
    /// </summary>
    /// <param name="from">Fecha inicial, por ejemplo 2026-11-20T00:00:00-05:00.</param>
    /// <param name="to">Fecha final (máximo 31 días después de la inicial).</param>
    /// <param name="cancellationToken">Token de cancelación de la petición.</param>
    [HttpGet("schedule")]
    [ProducesResponseType(typeof(IReadOnlyList<AvailableSessionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetSchedule(
        [FromQuery] DateTimeOffset? from = null,
        [FromQuery] DateTimeOffset? to = null,
        CancellationToken cancellationToken = default)
    {
        GetAvailableScheduleQuery query = new(User.IsInRole(Roles.MEMBER) ? User.GetEmail() : null)
        {
            From = from,
            To = to
        };

        IReadOnlyList<AvailableSessionDto> result = await mediator.Send(query, cancellationToken);

        return StatusCode(StatusCodes.Status200OK, result);
    }

    /// <summary>
    /// Reserva un cupo. El afiliado reserva para sí mismo; recepción y el administrador
    /// indican el cliente en <c>clientId</c>.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = $"{Roles.ADMIN},{Roles.RECEPTIONIST},{Roles.MEMBER}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Create([FromBody] CreateBookingRequest request, CancellationToken cancellationToken)
    {
        Guid? clientId = null;
        if (User.IsStaff())
        {
            clientId = request.ClientId
                ?? throw new ValidationException("Indica el cliente (clientId) para el que haces la reserva.");
        }

        Guid id = await mediator.Send(new CreateBookingCommand(request.SessionId, clientId), cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>
    /// Reservas del afiliado que inició sesión.
    /// </summary>
    /// <param name="upcomingOnly">Solo las reservas de sesiones que todavía no terminan.</param>
    /// <param name="status">Filtra por estado: Confirmed o Cancelled.</param>
    /// <param name="pageNumber">Número de página (mínimo 1).</param>
    /// <param name="pageSize">Reservas por página (máximo 50).</param>
    /// <param name="cancellationToken">Token de cancelación de la petición.</param>
    [HttpGet("me")]
    [Authorize(Roles = Roles.MEMBER)]
    [ProducesResponseType(typeof(PaginationResponse<BookingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetMine(
        [FromQuery] bool upcomingOnly = false,
        [FromQuery] BookingStatus? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = PaginationRequest.DEFAULT_PAGE_SIZE,
        CancellationToken cancellationToken = default)
    {
        GetMyBookingsQuery query = new(User.GetEmail())
        {
            Pagination = new PaginationRequest(pageNumber, pageSize),
            Status = status,
            UpcomingOnly = upcomingOnly
        };

        PaginationResponse<BookingDto> result = await mediator.Send(query, cancellationToken);

        return StatusCode(StatusCodes.Status200OK, result);
    }

    /// <summary>
    /// Todas las reservas, con filtros. Solo recepción y el administrador.
    /// </summary>
    /// <param name="clientId">Filtra por cliente.</param>
    /// <param name="sessionId">Filtra por sesión.</param>
    /// <param name="status">Filtra por estado: Confirmed o Cancelled.</param>
    /// <param name="upcomingOnly">Solo las reservas de sesiones que todavía no terminan.</param>
    /// <param name="pageNumber">Número de página (mínimo 1).</param>
    /// <param name="pageSize">Reservas por página (máximo 50).</param>
    /// <param name="cancellationToken">Token de cancelación de la petición.</param>
    [HttpGet]
    [Authorize(Roles = $"{Roles.ADMIN},{Roles.RECEPTIONIST}")]
    [ProducesResponseType(typeof(PaginationResponse<BookingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetList(
        [FromQuery] Guid? clientId = null,
        [FromQuery] Guid? sessionId = null,
        [FromQuery] BookingStatus? status = null,
        [FromQuery] bool upcomingOnly = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = PaginationRequest.DEFAULT_PAGE_SIZE,
        CancellationToken cancellationToken = default)
    {
        GetBookingListQuery query = new()
        {
            Pagination = new PaginationRequest(pageNumber, pageSize),
            ClientId = clientId,
            SessionId = sessionId,
            Status = status,
            UpcomingOnly = upcomingOnly
        };

        PaginationResponse<BookingDto> result = await mediator.Send(query, cancellationToken);

        return StatusCode(StatusCodes.Status200OK, result);
    }

    /// <summary>
    /// Detalle de una reserva. El afiliado solo puede ver las suyas.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        BookingDto booking = await mediator.Send(
            new GetBookingByIdQuery(id, User.GetEmail(), CanViewAnyBooking: User.IsStaff()), cancellationToken);

        return StatusCode(StatusCodes.Status200OK, booking);
    }

    /// <summary>
    /// Cancela una reserva y libera el cupo. El afiliado solo cancela las suyas.
    /// </summary>
    [HttpPatch("{id:guid}/cancel")]
    [Authorize(Roles = $"{Roles.ADMIN},{Roles.RECEPTIONIST},{Roles.MEMBER}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(
            new CancelBookingCommand(id, User.GetEmail(), CanCancelAnyBooking: User.IsStaff()), cancellationToken);

        return StatusCode(StatusCodes.Status204NoContent);
    }

    /// <summary>
    /// Inscritos de una sesión. El instructor solo ve los de sus sesiones (se consulta a Clases quién la dicta).
    /// </summary>
    [HttpGet("sessions/{sessionId:guid}")]
    [Authorize(Roles = $"{Roles.ADMIN},{Roles.RECEPTIONIST},{Roles.INSTRUCTOR}")]
    [ProducesResponseType(typeof(SessionBookingsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetSessionBookings(Guid sessionId, CancellationToken cancellationToken)
    {
        SessionBookingsDto result = await mediator.Send(
            new GetSessionBookingsQuery(sessionId, User.GetUserId(), CanViewAnySession: User.IsStaff()),
            cancellationToken);

        return StatusCode(StatusCodes.Status200OK, result);
    }
}
