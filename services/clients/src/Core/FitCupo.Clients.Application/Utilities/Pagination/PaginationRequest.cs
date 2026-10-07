namespace FitCupo.Clients.Application.Utilities.Pagination;

/// <summary>
/// Parámetros de paginación. Corrige los valores recibidos para que la página
/// sea como mínimo 1 y el tamaño no supere el máximo permitido.
/// </summary>
public sealed class PaginationRequest
{
    public const int DEFAULT_PAGE_SIZE = 15;
    public const int MAX_PAGE_SIZE = 50;

    public int PageNumber { get; }
    public int PageSize { get; }

    public PaginationRequest(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber < 1 ? 1 : pageNumber;
        PageSize = pageSize < 1 ? DEFAULT_PAGE_SIZE : Math.Min(pageSize, MAX_PAGE_SIZE);
    }

    /// <summary>
    /// Primera página con el tamaño por defecto.
    /// </summary>
    public static PaginationRequest Standard() => new(1, DEFAULT_PAGE_SIZE);
}
