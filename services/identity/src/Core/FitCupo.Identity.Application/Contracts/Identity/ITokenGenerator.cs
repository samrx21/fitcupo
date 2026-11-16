using FitCupo.Identity.Application.Models;

namespace FitCupo.Identity.Application.Contracts.Identity;

/// <summary>
/// Genera el JWT de un usuario autenticado.
/// </summary>
public interface ITokenGenerator
{
    AccessToken Generate(UserAccount user);
}
