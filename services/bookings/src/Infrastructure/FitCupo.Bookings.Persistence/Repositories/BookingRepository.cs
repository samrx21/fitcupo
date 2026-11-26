using FitCupo.Bookings.Application.Contracts.Repositories;
using FitCupo.Bookings.Application.Utilities.Pagination;
using FitCupo.Bookings.Domain.Entities.Bookings;
using FitCupo.Bookings.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FitCupo.Bookings.Persistence.Repositories;

public sealed class BookingRepository(BookingsDbContext context) : Repository<Booking>(context), IBookingRepository
{
    public async Task<int> CountConfirmedBySessionAsync(Guid sessionId, CancellationToken cancellationToken = default) =>
        await Confirmed().CountAsync(booking => booking.SessionId == sessionId, cancellationToken);

    public async Task<IReadOnlyDictionary<Guid, int>> CountConfirmedBySessionsAsync(
        IReadOnlyCollection<Guid> sessionIds, CancellationToken cancellationToken = default) =>
        await Confirmed()
            .Where(booking => sessionIds.Contains(booking.SessionId))
            .GroupBy(booking => booking.SessionId)
            .Select(group => new { SessionId = group.Key, Count = group.Count() })
            .ToDictionaryAsync(item => item.SessionId, item => item.Count, cancellationToken);

    public async Task<bool> HasConfirmedBookingAsync(
        Guid sessionId, Guid clientId, CancellationToken cancellationToken = default) =>
        await Confirmed().AnyAsync(
            booking => booking.SessionId == sessionId && booking.ClientId == clientId, cancellationToken);

    public async Task<bool> HasOverlappingBookingAsync(
        Guid clientId, DateTime startsAt, DateTime endsAt, CancellationToken cancellationToken = default) =>
        await Confirmed().AnyAsync(
            booking => booking.ClientId == clientId && booking.StartsAt < endsAt && startsAt < booking.EndsAt,
            cancellationToken);

    public async Task<IReadOnlyDictionary<Guid, Guid>> GetConfirmedBookingIdsAsync(
        string clientEmail, IReadOnlyCollection<Guid> sessionIds, CancellationToken cancellationToken = default) =>
        await Confirmed()
            .Where(booking => booking.ClientEmail == clientEmail && sessionIds.Contains(booking.SessionId))
            .ToDictionaryAsync(booking => booking.SessionId, booking => booking.Id, cancellationToken);

    public async Task<IReadOnlyList<Booking>> GetConfirmedBySessionAsync(
        Guid sessionId, CancellationToken cancellationToken = default) =>
        await Confirmed()
            .Where(booking => booking.SessionId == sessionId)
            .OrderBy(booking => booking.BookedAt)
            .ToListAsync(cancellationToken);

    public async Task<PaginationResponse<Booking>> GetPagedListAsync(
        PaginationRequest pagination,
        string? clientEmail,
        Guid? clientId,
        Guid? sessionId,
        BookingStatus? status,
        bool upcomingOnly,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Booking> query = Context.Bookings.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(clientEmail))
            query = query.Where(booking => booking.ClientEmail == clientEmail);

        if (clientId.HasValue)
            query = query.Where(booking => booking.ClientId == clientId.Value);

        if (sessionId.HasValue)
            query = query.Where(booking => booking.SessionId == sessionId.Value);

        if (status.HasValue)
            query = query.Where(booking => booking.Status == status.Value);

        if (upcomingOnly)
        {
            DateTime now = DateTime.UtcNow;
            query = query.Where(booking => booking.EndsAt > now);
        }

        query = query.OrderBy(booking => booking.StartsAt).ThenBy(booking => booking.Id);

        return await query.ToPagedListAsync(pagination, cancellationToken);
    }

    private IQueryable<Booking> Confirmed() =>
        Context.Bookings.AsNoTracking().Where(booking => booking.Status == BookingStatus.Confirmed);
}
