namespace FitCupo.Web.Models;

// Modelos de las respuestas de los microservicios, tal como llegan por el API Gateway.
// Las fechas vienen en UTC; la interfaz las muestra en la hora local del navegador.

public sealed record LoginResponse(string AccessToken, string TokenType, DateTime ExpiresAt);

public sealed class PagedResult<T>
{
    public List<T> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
    public bool HasPreviousPage { get; init; }
    public bool HasNextPage { get; init; }
}

public sealed record ServiceStatus(string Name, string Address, bool IsAvailable, long ElapsedMilliseconds);

/// <summary>Clase del catálogo (microservicio de Clases).</summary>
public sealed class GymClass
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public string Description { get; init; } = "";
    public string Category { get; init; } = "";
    public int DurationMinutes { get; init; }
    public int MaxCapacity { get; init; }
    public string Status { get; init; } = "";
    public bool IsActive => Status == "Active";
}

/// <summary>Sesión del horario (microservicio de Clases).</summary>
public sealed class ClassSession
{
    public Guid Id { get; init; }
    public Guid GymClassId { get; init; }
    public string GymClassName { get; init; } = "";
    public string Category { get; init; } = "";
    public Guid InstructorId { get; init; }
    public string InstructorName { get; init; } = "";
    public string Room { get; init; } = "";
    public DateTime StartsAt { get; init; }
    public DateTime EndsAt { get; init; }
    public int Capacity { get; init; }
    public string Status { get; init; } = "";
    public string? CancellationReason { get; init; }
    public bool IsScheduled => Status == "Scheduled";
}

/// <summary>Sesión con cupos disponibles (microservicio de Reservas, que consulta a Clases).</summary>
public sealed class AvailableSession
{
    public Guid SessionId { get; init; }
    public Guid GymClassId { get; init; }
    public string GymClassName { get; init; } = "";
    public string Category { get; init; } = "";
    public Guid InstructorId { get; init; }
    public string InstructorName { get; init; } = "";
    public string Room { get; init; } = "";
    public DateTime StartsAt { get; init; }
    public DateTime EndsAt { get; init; }
    public string Status { get; init; } = "";
    public string? CancellationReason { get; init; }
    public int Capacity { get; init; }
    public int ReservedSpots { get; init; }
    public int AvailableSpots { get; init; }
    public Guid? MyBookingId { get; init; }
    public bool IsScheduled => Status == "Scheduled";
    public bool HasStarted => StartsAt <= DateTime.UtcNow;
}

/// <summary>Reserva (microservicio de Reservas).</summary>
public sealed class Booking
{
    public Guid Id { get; init; }
    public Guid SessionId { get; init; }
    public string GymClassName { get; init; } = "";
    public string Room { get; init; } = "";
    public string InstructorName { get; init; } = "";
    public DateTime StartsAt { get; init; }
    public DateTime EndsAt { get; init; }
    public Guid ClientId { get; init; }
    public string ClientName { get; init; } = "";
    public string ClientEmail { get; init; } = "";
    public string Status { get; init; } = "";
    public DateTime BookedAt { get; init; }
    public DateTime? CancelledAt { get; init; }
    public bool IsConfirmed => Status == "Confirmed";
    public bool CanBeCancelled => IsConfirmed && StartsAt > DateTime.UtcNow;
}

/// <summary>Inscritos de una sesión (microservicio de Reservas).</summary>
public sealed class SessionBookings
{
    public Guid SessionId { get; init; }
    public string GymClassName { get; init; } = "";
    public string Room { get; init; } = "";
    public string InstructorName { get; init; } = "";
    public DateTime StartsAt { get; init; }
    public DateTime EndsAt { get; init; }
    public string SessionStatus { get; init; } = "";
    public int Capacity { get; init; }
    public int ReservedSpots { get; init; }
    public int AvailableSpots { get; init; }
    public List<Booking> Bookings { get; init; } = [];
}

/// <summary>Cliente en el listado (microservicio de Clientes).</summary>
public sealed class ClientListItem
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = "";
    public string DocumentType { get; init; } = "";
    public string DocumentNumber { get; init; } = "";
    public string Email { get; init; } = "";
    public string Phone { get; init; } = "";
    public string Status { get; init; } = "";
    public bool IsActive => Status == "Active";
}

/// <summary>Perfil completo de un cliente (microservicio de Clientes).</summary>
public sealed class ClientDetail
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = "";
    public string DocumentType { get; init; } = "";
    public string DocumentNumber { get; init; } = "";
    public string Email { get; init; } = "";
    public string Phone { get; init; } = "";
    public DateOnly BirthDate { get; init; }
    public string EmergencyContactName { get; init; } = "";
    public string EmergencyContactPhone { get; init; } = "";
    public string Status { get; init; } = "";
}

public sealed record CreatedResponse(Guid Id);
