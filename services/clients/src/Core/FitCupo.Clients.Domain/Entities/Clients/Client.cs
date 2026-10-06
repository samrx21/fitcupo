using FitCupo.Clients.Domain.Common.ValueObjects;
using FitCupo.Clients.Domain.Entities.Clients.ValueObjects;
using FitCupo.Clients.Domain.Exceptions;

namespace FitCupo.Clients.Domain.Entities.Clients;

/// <summary>
/// Cliente (afiliado) del gimnasio. Es la raíz del agregado: toda modificación
/// de sus datos pasa por sus métodos, que validan las reglas de negocio.
/// </summary>
public sealed class Client
{
    public const int NAME_MIN_LENGTH = 2;
    public const int NAME_MAX_LENGTH = 64;
    public const int MIN_AGE = 14;
    public const int MAX_AGE = 100;

    public Guid Id { get; private set; }
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public Document Document { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public PhoneNumber Phone { get; private set; } = null!;
    public DateOnly BirthDate { get; private set; }
    public EmergencyContact EmergencyContact { get; private set; } = null!;
    public ClientStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public string FullName => $"{FirstName} {LastName}";

    // Requerido por EF Core para materializar la entidad
    private Client() { }

    public Client(
        string firstName,
        string lastName,
        Document document,
        Email email,
        PhoneNumber phone,
        DateOnly birthDate,
        EmergencyContact emergencyContact)
    {
        string normalizedFirstName = firstName?.Trim() ?? string.Empty;
        string normalizedLastName = lastName?.Trim() ?? string.Empty;

        ApplyNameRules(normalizedFirstName, "nombre");
        ApplyNameRules(normalizedLastName, "apellido");
        ApplyDocumentRules(document);
        ApplyEmailRules(email);
        ApplyPhoneRules(phone);
        ApplyBirthDateRules(birthDate);
        ApplyEmergencyContactRules(emergencyContact, phone);

        Id = Guid.CreateVersion7();
        FirstName = normalizedFirstName;
        LastName = normalizedLastName;
        Document = document;
        Email = email;
        Phone = phone;
        BirthDate = birthDate;
        EmergencyContact = emergencyContact;
        Status = ClientStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdatePersonalInfo(string firstName, string lastName, DateOnly birthDate)
    {
        string normalizedFirstName = firstName?.Trim() ?? string.Empty;
        string normalizedLastName = lastName?.Trim() ?? string.Empty;

        EnsureIsActive();
        ApplyNameRules(normalizedFirstName, "nombre");
        ApplyNameRules(normalizedLastName, "apellido");
        ApplyBirthDateRules(birthDate);

        FirstName = normalizedFirstName;
        LastName = normalizedLastName;
        BirthDate = birthDate;
        MarkAsUpdated();
    }

    /// <summary>
    /// Actualiza los datos de contacto en un solo paso, porque la regla del teléfono
    /// de emergencia compara el teléfono del cliente con el de su contacto.
    /// </summary>
    public void UpdateContactInfo(Email email, PhoneNumber phone, EmergencyContact emergencyContact)
    {
        EnsureIsActive();
        ApplyEmailRules(email);
        ApplyPhoneRules(phone);
        ApplyEmergencyContactRules(emergencyContact, phone);

        Email = email;
        Phone = phone;
        EmergencyContact = emergencyContact;
        MarkAsUpdated();
    }

    public void Deactivate()
    {
        if (Status == ClientStatus.Inactive)
            throw new BusinessRuleException("El cliente ya se encuentra inactivo.");

        Status = ClientStatus.Inactive;
        MarkAsUpdated();
    }

    public void Activate()
    {
        if (Status == ClientStatus.Active)
            throw new BusinessRuleException("El cliente ya se encuentra activo.");

        Status = ClientStatus.Active;
        MarkAsUpdated();
    }

    private void EnsureIsActive()
    {
        if (Status == ClientStatus.Inactive)
            throw new BusinessRuleException("No se pueden modificar los datos de un cliente inactivo.");
    }

    private void MarkAsUpdated() => UpdatedAt = DateTime.UtcNow;

    private static void ApplyNameRules(string name, string field)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleException($"El {field} es requerido.");

        if (name.Length < NAME_MIN_LENGTH || name.Length > NAME_MAX_LENGTH)
            throw new BusinessRuleException(
                $"El {field} debe tener entre {NAME_MIN_LENGTH} y {NAME_MAX_LENGTH} caracteres.");
    }

    private static void ApplyDocumentRules(Document document)
    {
        if (document is null)
            throw new BusinessRuleException("El documento de identidad es requerido.");
    }

    private static void ApplyEmailRules(Email email)
    {
        if (email is null)
            throw new BusinessRuleException("El correo electrónico es requerido.");
    }

    private static void ApplyPhoneRules(PhoneNumber phone)
    {
        if (phone is null)
            throw new BusinessRuleException("El número de teléfono es requerido.");
    }

    private static void ApplyBirthDateRules(DateOnly birthDate)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (birthDate > today)
            throw new BusinessRuleException("La fecha de nacimiento no puede ser una fecha futura.");

        int age = today.Year - birthDate.Year;
        if (birthDate > today.AddYears(-age))
            age--;

        if (age < MIN_AGE)
            throw new BusinessRuleException($"El cliente debe tener al menos {MIN_AGE} años.");

        if (age > MAX_AGE)
            throw new BusinessRuleException($"La fecha de nacimiento no es válida: la edad supera {MAX_AGE} años.");
    }

    private static void ApplyEmergencyContactRules(EmergencyContact emergencyContact, PhoneNumber clientPhone)
    {
        if (emergencyContact is null)
            throw new BusinessRuleException("El contacto de emergencia es requerido.");

        if (emergencyContact.Phone == clientPhone)
            throw new BusinessRuleException(
                "El teléfono del contacto de emergencia debe ser distinto al teléfono del cliente.");
    }
}
