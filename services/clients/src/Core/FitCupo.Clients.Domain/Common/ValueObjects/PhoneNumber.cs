using FitCupo.Clients.Domain.Exceptions;

namespace FitCupo.Clients.Domain.Common.ValueObjects;

/// <summary>
/// Número de teléfono. Se aceptan espacios, guiones y paréntesis al escribirlo,
/// pero se guarda solo con dígitos (y el signo + inicial si lo tiene).
/// </summary>
public sealed record PhoneNumber
{
    public const int MIN_DIGITS = 7;
    public const int MAX_DIGITS = 15;

    public string Value { get; private set; } = null!;

    // Requerido por EF Core para materializar el objeto
    private PhoneNumber() { }

    public PhoneNumber(string value)
    {
        string normalized = Normalize(value);

        ApplyPhoneNumberRules(normalized);

        Value = normalized;
    }

    private static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        string trimmed = value.Trim();
        string digits = new(trimmed.Where(char.IsDigit).ToArray());

        return trimmed.StartsWith('+') ? $"+{digits}" : digits;
    }

    private static void ApplyPhoneNumberRules(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new BusinessRuleException("El número de teléfono es requerido.");

        int digitCount = phoneNumber.Count(char.IsDigit);

        if (digitCount < MIN_DIGITS || digitCount > MAX_DIGITS)
            throw new BusinessRuleException(
                $"El número de teléfono debe tener entre {MIN_DIGITS} y {MAX_DIGITS} dígitos.");
    }

    public override string ToString() => Value;
}
