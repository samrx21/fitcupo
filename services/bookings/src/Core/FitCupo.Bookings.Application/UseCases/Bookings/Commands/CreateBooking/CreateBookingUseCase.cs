using FitCupo.Bookings.Application.Contracts.Persistence;
using FitCupo.Bookings.Application.Contracts.Repositories;
using FitCupo.Bookings.Application.Contracts.Services;
using FitCupo.Bookings.Application.Exceptions;
using FitCupo.Bookings.Application.Models;
using FitCupo.Bookings.Application.Utilities.Mediator;
using FitCupo.Bookings.Domain.Entities.Bookings;
using FitCupo.Bookings.Domain.Entities.Bookings.ValueObjects;

namespace FitCupo.Bookings.Application.UseCases.Bookings.Commands.CreateBooking;

/// <summary>
/// Reserva un cupo. Antes de guardar consulta de forma síncrona a los otros microservicios:
/// a Clientes, para saber quién reserva y si está activo, y a Clases, para conocer la sesión y su cupo.
/// </summary>
public sealed class CreateBookingUseCase(
    IClientsService clientsService,
    IClassesService classesService,
    IBookingRepository bookingRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateBookingCommand, Guid>
{
    public async Task<Guid> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        // 1. Clientes: el afiliado se busca con su propio token; recepción indica el Id del cliente
        ClientInfo client = request.ClientId is { } clientId
            ? await clientsService.GetClientAsync(clientId, cancellationToken)
                ?? throw new NotFoundException($"No existe un cliente con el Id {clientId}.")
            : await clientsService.GetMyProfileAsync(cancellationToken)
                ?? throw new NotFoundException(
                    "Todavía no estás registrado como cliente del gimnasio. Acércate a recepción para completar tu registro.");

        // 2. Clases: la sesión debe existir; su estado, fecha y cupo se validan en el dominio
        ClassSessionInfo session = await classesService.GetSessionAsync(request.SessionId, cancellationToken)
            ?? throw new NotFoundException($"No existe una sesión con el Id {request.SessionId}.");

        // 3. Reglas que dependen de las reservas guardadas en este microservicio
        if (await bookingRepository.HasConfirmedBookingAsync(session.Id, client.Id, cancellationToken))
            throw new ConflictException($"{client.FullName} ya tiene una reserva en esta sesión.");

        if (await bookingRepository.HasOverlappingBookingAsync(client.Id, session.StartsAt, session.EndsAt, cancellationToken))
            throw new ConflictException($"{client.FullName} ya tiene otra clase reservada a esa misma hora.");

        int confirmedBookings = await bookingRepository.CountConfirmedBySessionAsync(session.Id, cancellationToken);

        Booking booking = Booking.Create(
            new BookableSession(
                session.Id,
                session.GymClassName,
                session.Room,
                session.InstructorId,
                session.InstructorName,
                DateTime.SpecifyKind(session.StartsAt, DateTimeKind.Utc),
                DateTime.SpecifyKind(session.EndsAt, DateTimeKind.Utc),
                session.Capacity,
                session.IsScheduled),
            new BookingClient(client.Id, client.FullName, client.Email, client.IsActive),
            confirmedBookings);

        try
        {
            await bookingRepository.AddAsync(booking, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }

        return booking.Id;
    }
}
