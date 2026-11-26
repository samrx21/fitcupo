using FitCupo.Bookings.Domain.Entities.Bookings.ValueObjects;
using FitCupo.Bookings.Domain.Exceptions;

namespace FitCupo.Bookings.Domain.Entities.Bookings;

/// <summary>
/// Reserva de un cupo en una sesión. Referencia la sesión (microservicio de Clases) y el
/// cliente (microservicio de Clientes) por su Id, y guarda una copia de sus datos principales.
/// </summary>
public sealed class Booking
{
    public const int CLIENT_NAME_MAX_LENGTH = 130;
    public const int CLIENT_EMAIL_MAX_LENGTH = 150;
    public const int CLASS_NAME_MAX_LENGTH = 80;
    public const int ROOM_MAX_LENGTH = 40;
    public const int INSTRUCTOR_NAME_MAX_LENGTH = 130;

    public Guid Id { get; private set; }

    // Sesión reservada (copia de los datos de Clases)
    public Guid SessionId { get; private set; }
    public string GymClassName { get; private set; } = null!;
    public string Room { get; private set; } = null!;
    public Guid InstructorId { get; private set; }
    public string InstructorName { get; private set; } = null!;
    public DateTime StartsAt { get; private set; }
    public DateTime EndsAt { get; private set; }

    // Cliente que reserva (copia de los datos de Clientes)
    public Guid ClientId { get; private set; }
    public string ClientName { get; private set; } = null!;
    public string ClientEmail { get; private set; } = null!;

    public BookingStatus Status { get; private set; }
    public DateTime BookedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }

    // Requerido por EF Core para materializar la entidad
    private Booking() { }

    /// <summary>
    /// Reserva un cupo. <paramref name="confirmedBookings"/> es el número de reservas
    /// confirmadas que ya tiene la sesión; con él se valida que quede cupo.
    /// </summary>
    public static Booking Create(BookableSession session, BookingClient client, int confirmedBookings)
    {
        DateTime now = DateTime.UtcNow;

        if (!client.IsActive)
            throw new BusinessRuleException(
                $"El cliente {client.FullName} está inactivo y no puede reservar clases.");

        if (!session.IsScheduled)
            throw new BusinessRuleException("La sesión fue cancelada y no admite reservas.");

        if (session.StartsAt <= now)
            throw new BusinessRuleException("La sesión ya empezó; solo se reservan sesiones futuras.");

        if (confirmedBookings >= session.Capacity)
            throw new BusinessRuleException(
                $"La sesión de {session.GymClassName} no tiene cupos disponibles ({session.Capacity} de {session.Capacity} ocupados).");

        return new Booking
        {
            Id = Guid.CreateVersion7(),
            SessionId = session.SessionId,
            GymClassName = session.GymClassName,
            Room = session.Room,
            InstructorId = session.InstructorId,
            InstructorName = session.InstructorName,
            StartsAt = session.StartsAt,
            EndsAt = session.EndsAt,
            ClientId = client.ClientId,
            ClientName = client.FullName,
            ClientEmail = client.Email,
            Status = BookingStatus.Confirmed,
            BookedAt = now
        };
    }

    public bool IsConfirmed => Status == BookingStatus.Confirmed;

    public bool BelongsTo(string email) =>
        string.Equals(ClientEmail, email?.Trim(), StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Cancela la reserva y libera el cupo. Solo antes de que empiece la sesión.
    /// </summary>
    public void Cancel()
    {
        DateTime now = DateTime.UtcNow;

        if (Status == BookingStatus.Cancelled)
            throw new BusinessRuleException("La reserva ya está cancelada.");

        if (StartsAt <= now)
            throw new BusinessRuleException("No se puede cancelar la reserva de una sesión que ya empezó.");

        Status = BookingStatus.Cancelled;
        CancelledAt = now;
    }
}
