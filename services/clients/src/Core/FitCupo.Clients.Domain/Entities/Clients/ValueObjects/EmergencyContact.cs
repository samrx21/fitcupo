using FitCupo.Clients.Domain.Common.ValueObjects;
using FitCupo.Clients.Domain.Exceptions;

namespace FitCupo.Clients.Domain.Entities.Clients.ValueObjects;

/// <summary>
/// Persona a la que el gimnasio contacta si el cliente tiene una emergencia.
/// </summary>
public sealed record EmergencyContact
{
    public const int NAME_MIN_LENGTH = 3;
    public const int NAME_MAX_LENGTH = 64;

    public string Name { get; private set; } = null!;
    public PhoneNumber Phone { get; private set; } = null!;

    // Requerido por EF Core para materializar el objeto
    private EmergencyContact() { }

    public EmergencyContact(string name, PhoneNumber phone)
    {
        string normalized = name?.Trim() ?? string.Empty;

        ApplyNameRules(normalized);
        ApplyPhoneRules(phone);

        Name = normalized;
        Phone = phone;
    }

    private static void ApplyNameRules(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleException("El nombre del contacto de emergencia es requerido.");

        if (name.Length < NAME_MIN_LENGTH || name.Length > NAME_MAX_LENGTH)
            throw new BusinessRuleException(
                $"El nombre del contacto de emergencia debe tener entre {NAME_MIN_LENGTH} y {NAME_MAX_LENGTH} caracteres.");
    }

    private static void ApplyPhoneRules(PhoneNumber phone)
    {
        if (phone is null)
            throw new BusinessRuleException("El teléfono del contacto de emergencia es requerido.");
    }
}
