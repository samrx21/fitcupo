namespace FitCupo.Identity.Application.Constants;

/// <summary>
/// Roles de FitCupo. Identity los crea con el seeder y los incluye en el JWT;
/// los demás microservicios los usan para autorizar sus endpoints.
/// </summary>
public static class Roles
{
    public const string ADMIN = "Admin";
    public const string RECEPTIONIST = "Receptionist";
    public const string INSTRUCTOR = "Instructor";
    public const string MEMBER = "Member";
}
