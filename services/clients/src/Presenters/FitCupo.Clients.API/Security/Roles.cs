namespace FitCupo.Clients.API.Security;

/// <summary>
/// Roles que emite Identity en el claim "role" del JWT.
/// </summary>
public static class Roles
{
    public const string ADMIN = "Admin";
    public const string RECEPTIONIST = "Receptionist";
    public const string INSTRUCTOR = "Instructor";
    public const string MEMBER = "Member";
}
