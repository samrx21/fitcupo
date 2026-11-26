using System.Globalization;
using FitCupo.Bookings.Application.Contracts.Services;
using FitCupo.Bookings.Application.Models;

namespace FitCupo.Bookings.Infrastructure.Services;

/// <summary>
/// Cliente HTTP del microservicio de Clases.
/// </summary>
public sealed class ClassesService(HttpClient httpClient) : RemoteServiceClient(httpClient, "Clases"), IClassesService
{
    // Tamaño máximo de página que acepta Clases
    private const int PAGE_SIZE = 50;

    public Task<ClassSessionInfo?> GetSessionAsync(Guid sessionId, CancellationToken cancellationToken = default) =>
        GetAsync<ClassSessionInfo>($"api/sessions/{sessionId}", cancellationToken);

    public async Task<IReadOnlyList<ClassSessionInfo>> GetScheduleAsync(
        DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
    {
        List<ClassSessionInfo> sessions = [];
        string fromText = Uri.EscapeDataString(from.ToString("O", CultureInfo.InvariantCulture));
        string toText = Uri.EscapeDataString(to.ToString("O", CultureInfo.InvariantCulture));
        int pageNumber = 1;
        PageResponse? page;

        // Clases pagina el horario: se piden todas las páginas del rango
        do
        {
            page = await GetAsync<PageResponse>(
                $"api/sessions?from={fromText}&to={toText}&pageNumber={pageNumber}&pageSize={PAGE_SIZE}",
                cancellationToken);

            if (page is null)
                break;

            sessions.AddRange(page.Items);
            pageNumber++;
        }
        while (page.HasNextPage);

        return sessions;
    }

    private sealed class PageResponse
    {
        public List<ClassSessionInfo> Items { get; init; } = [];
        public bool HasNextPage { get; init; }
    }
}
