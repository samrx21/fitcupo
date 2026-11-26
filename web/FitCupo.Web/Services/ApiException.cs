namespace FitCupo.Web.Services;

/// <summary>
/// Error que devolvió el Gateway o un microservicio, con el título y el detalle de su ProblemDetails.
/// </summary>
public sealed class ApiException(int statusCode, string title, string detail) : Exception(detail)
{
    public int StatusCode { get; } = statusCode;
    public string Title { get; } = title;
    public string Detail { get; } = detail;
}
