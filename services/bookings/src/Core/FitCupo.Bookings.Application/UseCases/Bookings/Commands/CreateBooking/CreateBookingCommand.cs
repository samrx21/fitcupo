using FitCupo.Bookings.Application.Utilities.Mediator;

namespace FitCupo.Bookings.Application.UseCases.Bookings.Commands.CreateBooking;

/// <summary>
/// Comando para reservar un cupo en una sesión. Devuelve el Id de la reserva.
/// Si <paramref name="ClientId"/> es null, reserva el afiliado que inició sesión;
/// recepción y el administrador indican el cliente para el que reservan.
/// </summary>
public sealed record CreateBookingCommand(Guid SessionId, Guid? ClientId) : IRequest<Guid>;
