using FitCupo.Bookings.Application.Contracts.Repositories;
using FitCupo.Bookings.Application.Exceptions;
using FitCupo.Bookings.Application.Utilities.Mediator;
using FitCupo.Bookings.Domain.Entities.Bookings;

namespace FitCupo.Bookings.Application.UseCases.Bookings.Queries.GetBookingById;

public sealed class GetBookingByIdUseCase(IBookingRepository bookingRepository)
    : IRequestHandler<GetBookingByIdQuery, BookingDto>
{
    public async Task<BookingDto> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
    {
        Booking booking = await bookingRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"No existe una reserva con el Id {request.Id}.");

        if (!request.CanViewAnyBooking && !booking.BelongsTo(request.RequesterEmail))
            throw new ForbiddenException("Solo puedes consultar tus propias reservas.");

        return booking.ToDto();
    }
}
