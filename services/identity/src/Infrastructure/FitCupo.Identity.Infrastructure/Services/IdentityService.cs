using FitCupo.Identity.Application.Contracts.Identity;
using FitCupo.Identity.Application.Exceptions;
using FitCupo.Identity.Application.Models;
using FitCupo.Identity.Infrastructure.Entities;
using FitCupo.Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FitCupo.Identity.Infrastructure.Services;

/// <summary>
/// Implementa las operaciones de usuarios con los servicios de ASP.NET Core Identity.
/// </summary>
public sealed class IdentityService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    FitCupoIdentityDbContext context) : IIdentityService
{
    private const string INVALID_CREDENTIALS_MESSAGE = "El correo o la contraseña no son correctos.";

    public async Task<UserAccount> AuthenticateAsync(
        string email, string password, CancellationToken cancellationToken = default)
    {
        ApplicationUser user = await userManager.FindByEmailAsync(email)
            ?? throw new InvalidCredentialsException(INVALID_CREDENTIALS_MESSAGE);

        // Compara la contraseña con el hash guardado y cuenta los intentos fallidos para bloquear la cuenta
        SignInResult result = await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);

        if (result.IsLockedOut)
            throw new InvalidCredentialsException(
                "La cuenta está bloqueada temporalmente por varios intentos fallidos. Intenta más tarde.");

        if (!result.Succeeded)
            throw new InvalidCredentialsException(INVALID_CREDENTIALS_MESSAGE);

        IList<string> roles = await userManager.GetRolesAsync(user);

        return new UserAccount(user.Id, user.Email!, user.FirstName, user.LastName, roles.Order().ToList());
    }

    public async Task<UserAccount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await UsersWithRoles()
            .Where(user => user.Id == id)
            .Select(ToUserAccount)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<UserAccount>> GetListAsync(
        string? role, CancellationToken cancellationToken = default)
    {
        IQueryable<ApplicationUser> query = UsersWithRoles();

        if (!string.IsNullOrWhiteSpace(role))
        {
            string normalizedRole = role.Trim().ToUpperInvariant();
            query = query.Where(user => user.UserRoles.Any(userRole => userRole.Role.NormalizedName == normalizedRole));
        }

        return await query
            .OrderBy(user => user.LastName)
            .ThenBy(user => user.FirstName)
            .Select(ToUserAccount)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Consulta de solo lectura que usa la relación UserRoles para traer los roles en la misma consulta.
    /// </summary>
    private IQueryable<ApplicationUser> UsersWithRoles() => context.Users.AsNoTracking();

    private static readonly System.Linq.Expressions.Expression<Func<ApplicationUser, UserAccount>> ToUserAccount =
        user => new UserAccount(
            user.Id,
            user.Email!,
            user.FirstName,
            user.LastName,
            user.UserRoles.Select(userRole => userRole.Role.Name!).OrderBy(name => name).ToList());
}
