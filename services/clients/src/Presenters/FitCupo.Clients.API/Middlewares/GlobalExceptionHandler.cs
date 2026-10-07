using FitCupo.Clients.Application.Exceptions;
using FitCupo.Clients.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FitCupo.Clients.API.Middlewares;

/// <summary>
/// Traduce las excepciones de las capas internas a respuestas HTTP estándar
/// (ProblemDetails), para que el cliente reciba siempre el mismo formato de error.
/// </summary>
public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        (int statusCode, string title) = exception switch
        {
            BusinessRuleException => (StatusCodes.Status400BadRequest, "Regla de negocio incumplida"),
            NotFoundException => (StatusCodes.Status404NotFound, "Recurso no encontrado"),
            ConflictException => (StatusCodes.Status409Conflict, "Conflicto con datos existentes"),
            _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Error no controlado al procesar {Path}", httpContext.Request.Path);

        httpContext.Response.StatusCode = statusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                // Los errores inesperados no exponen detalles internos al cliente
                Detail = statusCode == StatusCodes.Status500InternalServerError
                    ? "Ocurrió un error inesperado. Intenta de nuevo más tarde."
                    : exception.Message
            }
        });
    }
}
