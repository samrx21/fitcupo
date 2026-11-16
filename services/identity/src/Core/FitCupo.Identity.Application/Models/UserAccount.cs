namespace FitCupo.Identity.Application.Models;

/// <summary>
/// Datos de un usuario de Identity con sus roles. Aplicación trabaja con este modelo
/// y no con la entidad de ASP.NET Core Identity, que pertenece a Infraestructura.
/// </summary>
public sealed record UserAccount(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    IReadOnlyList<string> Roles)
{
    public string FullName => $"{FirstName} {LastName}";
}
