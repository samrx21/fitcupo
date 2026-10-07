namespace FitCupo.Clients.API.Requests;

/// <summary>
/// Cuerpo de la petición para actualizar un cliente. El Id viaja en la ruta.
/// </summary>
public sealed record UpdateClientRequest(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    DateOnly BirthDate,
    string EmergencyContactName,
    string EmergencyContactPhone);
