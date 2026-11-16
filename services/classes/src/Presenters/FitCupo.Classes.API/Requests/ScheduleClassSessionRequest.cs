namespace FitCupo.Classes.API.Requests;

/// <summary>
/// Cuerpo de la petición para programar una sesión. No incluye el instructor:
/// se toma del JWT del usuario autenticado.
/// </summary>
/// <param name="GymClassId">Clase que se va a dictar.</param>
/// <param name="Room">Salón donde se dicta la sesión.</param>
/// <param name="StartsAt">Fecha y hora de inicio con su zona horaria, por ejemplo 2026-11-20T18:00:00-05:00.</param>
/// <param name="Capacity">Cupos de la sesión. Si se omite, se usa el aforo máximo de la clase.</param>
public sealed record ScheduleClassSessionRequest(
    Guid GymClassId,
    string Room,
    DateTimeOffset StartsAt,
    int? Capacity);
