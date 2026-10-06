using System.Text.RegularExpressions;
using FitCupo.Clients.Domain.Exceptions;

namespace FitCupo.Clients.Domain.Common.ValueObjects;

/// <summary>
/// Correo electrónico. Se guarda en minúsculas y sin espacios para que
/// dos correos escritos de forma distinta se consideren el mismo.
/// </summary>
public sealed record Email
{
    public const int MAX_LENGTH = 128;

    private static readonly Regex EmailPattern = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public string Value { get; private set; } = null!;

    // Requerido por EF Core para materializar el objeto
    private Email() { }

    public Email(string value)
    {
        string normalized = value?.Trim().ToLowerInvariant() ?? string.Empty;

        ApplyEmailRules(normalized);

        Value = normalized;
    }

    private static void ApplyEmailRules(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new BusinessRuleException("El correo electrónico es requerido.");

        if (email.Length > MAX_LENGTH)
            throw new BusinessRuleException($"El correo electrónico no puede superar {MAX_LENGTH} caracteres.");

        if (!EmailPattern.IsMatch(email))
            throw new BusinessRuleException("El correo electrónico no tiene un formato válido.");
    }

    public override string ToString() => Value;
}
