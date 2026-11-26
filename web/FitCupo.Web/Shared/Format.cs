using System.Globalization;

namespace FitCupo.Web.Shared;

/// <summary>
/// Formatos en español para fechas, categorías y estados.
/// </summary>
public static class Format
{
    private static readonly CultureInfo Spanish = new("es-CO");

    /// <summary>
    /// Convierte una fecha UTC de la API a la hora local, por ejemplo "jue 26 nov · 7:00 a. m.".
    /// </summary>
    public static string DateTime(DateTime utc) =>
        ToLocal(utc).ToString("ddd d MMM · h:mm tt", Spanish);

    public static string Time(DateTime utc) => ToLocal(utc).ToString("h:mm tt", Spanish);

    public static string Day(DateTime utc) => ToLocal(utc).ToString("dddd d 'de' MMMM", Spanish);

    public static DateTime ToLocal(DateTime utc) =>
        System.DateTime.SpecifyKind(utc, DateTimeKind.Utc).ToLocalTime();

    public static string Category(string category) => category switch
    {
        "Cycling" => "Ciclismo",
        "Yoga" => "Yoga",
        "Functional" => "Funcional",
        "Strength" => "Fuerza",
        "Dance" => "Baile",
        "Cardio" => "Cardio",
        _ => category
    };

    public static string DocumentType(string type) => type switch
    {
        "CitizenshipCard" => "C.C.",
        "ForeignerId" => "C.E.",
        "IdentityCard" => "T.I.",
        "Passport" => "Pasaporte",
        _ => type
    };
}
