using Microsoft.AspNetCore.Identity;

namespace FitCupo.Identity.Infrastructure.Seeders;

internal static class SeederResult
{
    /// <summary>
    /// Detiene el arranque si Identity rechaza un dato inicial, mostrando el motivo.
    /// </summary>
    public static void EnsureSucceeded(IdentityResult result, string operation)
    {
        if (result.Succeeded)
            return;

        string errors = string.Join(" ", result.Errors.Select(error => error.Description));
        throw new InvalidOperationException($"El seeder no pudo {operation}: {errors}");
    }
}
