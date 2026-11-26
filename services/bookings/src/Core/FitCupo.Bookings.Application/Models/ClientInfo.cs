namespace FitCupo.Bookings.Application.Models;

/// <summary>
/// Cliente tal como lo responde el microservicio de Clientes (GET /api/clients/{id} o /me).
/// </summary>
public sealed class ClientInfo
{
    public const string ACTIVE_STATUS = "Active";

    public Guid Id { get; init; }
    public string FullName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Status { get; init; } = null!;

    public bool IsActive => Status == ACTIVE_STATUS;
}
