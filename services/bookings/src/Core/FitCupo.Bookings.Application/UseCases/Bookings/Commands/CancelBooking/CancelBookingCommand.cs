using FitCupo.Bookings.Application.Utilities.Mediator;

namespace FitCupo.Bookings.Application.UseCases.Bookings.Commands.CancelBooking;

/// <summary>
/// Comando para cancelar una reserva. <paramref name="CanCancelAnyBooking"/> es verdadero para
/// recepción y el administrador; un afiliado solo cancela sus reservas (se compara su correo).
/// </summary>
public sealed record CancelBookingCommand(Guid Id, string RequesterEmail, bool CanCancelAnyBooking) : IRequest;
