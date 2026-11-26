namespace FitCupo.Web.Auth;

/// <summary>
/// Roles que emite Identity en el claim "role" del JWT, con su nombre en español para la interfaz.
/// </summary>
public static class Roles
{
    public const string ADMIN = "Admin";
    public const string RECEPTIONIST = "Receptionist";
    public const string INSTRUCTOR = "Instructor";
    public const string MEMBER = "Member";

    public const string STAFF = $"{ADMIN},{RECEPTIONIST}";

    public static string ToSpanish(string role) => role switch
    {
        ADMIN => "Administrador",
        RECEPTIONIST => "Recepción",
        INSTRUCTOR => "Instructor",
        MEMBER => "Afiliado",
        _ => role
    };
}
