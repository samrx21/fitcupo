using FitCupo.Clients.Domain.Exceptions;

namespace FitCupo.Clients.Domain.Entities.Clients.ValueObjects;

/// <summary>
/// Documento de identidad del cliente: tipo y número.
/// El pasaporte admite letras y números; los demás tipos solo números.
/// </summary>
public sealed record Document
{
    public const int MIN_LENGTH = 5;
    public const int MAX_LENGTH = 15;

    public DocumentType Type { get; private set; }
    public string Number { get; private set; } = null!;

    // Requerido por EF Core para materializar el objeto
    private Document() { }

    public Document(DocumentType type, string number)
    {
        string normalized = number?.Trim().ToUpperInvariant() ?? string.Empty;

        ApplyTypeRules(type);
        ApplyNumberRules(type, normalized);

        Type = type;
        Number = normalized;
    }

    private static void ApplyTypeRules(DocumentType type)
    {
        if (!Enum.IsDefined(type))
            throw new BusinessRuleException("El tipo de documento no es válido.");
    }

    private static void ApplyNumberRules(DocumentType type, string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new BusinessRuleException("El número de documento es requerido.");

        if (number.Length < MIN_LENGTH || number.Length > MAX_LENGTH)
            throw new BusinessRuleException(
                $"El número de documento debe tener entre {MIN_LENGTH} y {MAX_LENGTH} caracteres.");

        bool isValid = type == DocumentType.Passport
            ? number.All(char.IsLetterOrDigit)
            : number.All(char.IsDigit);

        if (!isValid)
            throw new BusinessRuleException(type == DocumentType.Passport
                ? "El número de pasaporte solo puede contener letras y números."
                : "El número de documento solo puede contener números.");
    }

    public override string ToString() => $"{Type} {Number}";
}
