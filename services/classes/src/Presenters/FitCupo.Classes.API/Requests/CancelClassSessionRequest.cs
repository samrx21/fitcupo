namespace FitCupo.Classes.API.Requests;

/// <summary>
/// Cuerpo de la petición para cancelar una sesión.
/// </summary>
public sealed record CancelClassSessionRequest(string Reason);
