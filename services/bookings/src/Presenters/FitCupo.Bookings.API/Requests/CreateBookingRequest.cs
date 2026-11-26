namespace FitCupo.Bookings.API.Requests;

/// <summary>
/// Cuerpo de POST /api/bookings. El afiliado solo envía la sesión; recepción y el
/// administrador envían también el cliente para el que reservan.
/// </summary>
public sealed record CreateBookingRequest(Guid SessionId, Guid? ClientId);
