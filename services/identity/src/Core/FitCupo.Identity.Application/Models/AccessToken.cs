namespace FitCupo.Identity.Application.Models;

/// <summary>
/// JWT firmado y la fecha (UTC) en la que deja de ser válido.
/// </summary>
public sealed record AccessToken(string Token, DateTime ExpiresAt);
