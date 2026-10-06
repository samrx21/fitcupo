namespace FitCupo.Clients.Application.Utilities.Pagination;

/// <summary>
/// Resultado paginado: los elementos de la página solicitada y los datos
/// necesarios para navegar entre páginas.
/// </summary>
public sealed class PaginationResponse<T>
{
    public required IReadOnlyList<T> Items { get; init; }
    public required int TotalCount { get; init; }
    public required int PageNumber { get; init; }
    public required int PageSize { get; init; }

    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;

    public static PaginationResponse<T> Create(IReadOnlyList<T> items, int totalCount, PaginationRequest request) =>
        new()
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
}
