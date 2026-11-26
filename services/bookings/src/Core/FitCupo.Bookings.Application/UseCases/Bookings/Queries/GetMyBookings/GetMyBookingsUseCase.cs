using FitCupo.Bookings.Application.Contracts.Repositories;
using FitCupo.Bookings.Application.Utilities.Mediator;
using FitCupo.Bookings.Application.Utilities.Pagination;
using FitCupo.Bookings.Domain.Entities.Bookings;

namespace FitCupo.Bookings.Application.UseCases.Bookings.Queries.GetMyBookings;

public sealed class GetMyBookingsUseCase(IBookingRepository bookingRepository)
    : IRequestHandler<GetMyBookingsQuery, PaginationResponse<BookingDto>>
{
    public async Task<PaginationResponse<BookingDto>> Handle(GetMyBookingsQuery request, CancellationToken cancellationToken)
    {
        PaginationResponse<Booking> page = await bookingRepository.GetPagedListAsync(
            request.Pagination,
            clientEmail: request.Email.Trim().ToLowerInvariant(),
            clientId: null,
            sessionId: null,
            request.Status,
            request.UpcomingOnly,
            cancellationToken);

        return page.ToDto();
    }
}
