namespace FitCupo.Bookings.Domain.Entities.Bookings;

/// <summary>
/// Estado de una reserva. Las reservas no se eliminan: se cancelan para conservar el historial.
/// </summary>
public enum BookingStatus
{
    Confirmed = 1,
    Cancelled = 2
}
