using FitCupo.Bookings.Application.Exceptions;
using FitCupo.Bookings.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FitCupo.Bookings.API.Middlewares;

/// <summary>
/// Traduce las excepciones de las capas internas a respuestas HTTP estándar
/// (ProblemDetails), para que el cliente reciba siempre el mismo formato de error.
/// </summary>
public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    // Errores de SQL Server por llave o índice único duplicado
    private static readonly int[] UniqueViolationErrors = [2601, 2627];

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        (int statusCode, string title, string detail) = exception switch
        {
            BusinessRuleException => (StatusCodes.Status400BadRequest, "Regla de negocio incumplida", exception.Message),
            ValidationException => (StatusCodes.Status400BadRequest, "Parámetros inválidos", exception.Message),
            ForbiddenException => (StatusCodes.Status403Forbidden, "Acceso denegado", exception.Message),
            NotFoundException => (StatusCodes.Status404NotFound, "Recurso no encontrado", exception.Message),
            ConflictException => (StatusCodes.Status409Conflict, "Conflicto con datos existentes", exception.Message),
            DbUpdateException { InnerException: SqlException sql } when UniqueViolationErrors.Contains(sql.Number) =>
                (StatusCodes.Status409Conflict, "Conflicto con datos existentes", "El cliente ya tiene una reserva en esta sesión."),
            ServiceUnavailableException => (StatusCodes.Status503ServiceUnavailable, "Servicio no disponible", exception.Message),
            // Los errores inesperados no exponen detalles internos al cliente
            _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor",
                "Ocurrió un error inesperado. Intenta de nuevo más tarde.")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Error no controlado al procesar {Path}", httpContext.Request.Path);
        else if (statusCode == StatusCodes.Status503ServiceUnavailable)
            logger.LogWarning("Reservas no pudo completar {Path}: {Detail}", httpContext.Request.Path, detail);

        httpContext.Response.StatusCode = statusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails { Status = statusCode, Title = title, Detail = detail }
        });
    }
}
