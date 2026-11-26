using FitCupo.Bookings.Application.Contracts.Persistence;
using FitCupo.Bookings.Application.Contracts.Repositories;
using FitCupo.Bookings.Application.Exceptions;
using FitCupo.Bookings.Application.Utilities.Mediator;
using FitCupo.Bookings.Domain.Entities.Bookings;

namespace FitCupo.Bookings.Application.UseCases.Bookings.Commands.CancelBooking;

public sealed class CancelBookingUseCase(IBookingRepository bookingRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CancelBookingCommand>
{
    public async Task Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        Booking booking = await bookingRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"No existe una reserva con el Id {request.Id}.");

        // Autorización por usuario: el afiliado solo puede cancelar sus propias reservas
        if (!request.CanCancelAnyBooking && !booking.BelongsTo(request.RequesterEmail))
            throw new ForbiddenException("Solo puedes cancelar tus propias reservas.");

        booking.Cancel();

        await bookingRepository.UpdateAsync(booking, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
