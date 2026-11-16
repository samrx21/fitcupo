namespace FitCupo.Identity.Application.UseCases.Users;

/// <summary>
/// Datos públicos de un usuario. Nunca incluye la contraseña ni su hash.
/// </summary>
public sealed class UserDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = null!;
    public string FullName { get; init; } = null!;
    public IReadOnlyList<string> Roles { get; init; } = [];
}
