using FitCupo.Bookings.Application.Contracts.Repositories;
using FitCupo.Bookings.Application.Utilities.Mediator;
using FitCupo.Bookings.Application.Utilities.Pagination;
using FitCupo.Bookings.Domain.Entities.Bookings;

namespace FitCupo.Bookings.Application.UseCases.Bookings.Queries.GetBookingList;

public sealed class GetBookingListUseCase(IBookingRepository bookingRepository)
    : IRequestHandler<GetBookingListQuery, PaginationResponse<BookingDto>>
{
    public async Task<PaginationResponse<BookingDto>> Handle(GetBookingListQuery request, CancellationToken cancellationToken)
    {
        PaginationResponse<Booking> page = await bookingRepository.GetPagedListAsync(
            request.Pagination,
            clientEmail: null,
            request.ClientId,
            request.SessionId,
            request.Status,
            request.UpcomingOnly,
            cancellationToken);

        return page.ToDto();
    }
}
