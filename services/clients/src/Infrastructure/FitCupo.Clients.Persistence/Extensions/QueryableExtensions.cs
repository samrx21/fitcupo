using FitCupo.Clients.Application.Utilities.Pagination;
using Microsoft.EntityFrameworkCore;

namespace FitCupo.Clients.Persistence.Extensions;

internal static class QueryableExtensions
{
    /// <summary>
    /// Ejecuta la consulta paginada: cuenta el total de registros y trae solo los
    /// de la página solicitada. La consulta debe llegar ordenada.
    /// </summary>
    public static async Task<PaginationResponse<T>> ToPagedListAsync<T>(
        this IQueryable<T> query,
        PaginationRequest request,
        CancellationToken cancellationToken = default)
    {
        int totalCount = await query.CountAsync(cancellationToken);

        List<T> items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return PaginationResponse<T>.Create(items, totalCount, request);
    }
}
