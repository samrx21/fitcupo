using FitCupo.Classes.Domain.Exceptions;

namespace FitCupo.Classes.Domain.Entities.GymClasses;

/// <summary>
/// Clase grupal del catálogo del gimnasio (por ejemplo, Spinning o Yoga). Es la raíz
/// del agregado: define la duración y el aforo máximo que usan sus sesiones.
/// </summary>
public sealed class GymClass
{
    public const int NAME_MIN_LENGTH = 3;
    public const int NAME_MAX_LENGTH = 60;
    public const int DESCRIPTION_MAX_LENGTH = 300;
    public const int MIN_DURATION_MINUTES = 15;
    public const int MAX_DURATION_MINUTES = 180;
    public const int MIN_CAPACITY = 1;
    public const int MAX_CAPACITY = 60;

    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public ClassCategory Category { get; private set; }
    public int DurationMinutes { get; private set; }
    public int MaxCapacity { get; private set; }
    public GymClassStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public bool IsActive => Status == GymClassStatus.Active;

    // Requerido por EF Core para materializar la entidad
    private GymClass() { }

    public GymClass(string name, string description, ClassCategory category, int durationMinutes, int maxCapacity)
    {
        string normalizedName = name?.Trim() ?? string.Empty;
        string normalizedDescription = description?.Trim() ?? string.Empty;

        ApplyNameRules(normalizedName);
        ApplyDescriptionRules(normalizedDescription);
        ApplyCategoryRules(category);
        ApplyDurationRules(durationMinutes);
        ApplyCapacityRules(maxCapacity);

        Id = Guid.CreateVersion7();
        Name = normalizedName;
        Description = normalizedDescription;
        Category = category;
        DurationMinutes = durationMinutes;
        MaxCapacity = maxCapacity;
        Status = GymClassStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string description, ClassCategory category, int durationMinutes, int maxCapacity)
    {
        string normalizedName = name?.Trim() ?? string.Empty;
        string normalizedDescription = description?.Trim() ?? string.Empty;

        EnsureIsActive();
        ApplyNameRules(normalizedName);
        ApplyDescriptionRules(normalizedDescription);
        ApplyCategoryRules(category);
        ApplyDurationRules(durationMinutes);
        ApplyCapacityRules(maxCapacity);

        Name = normalizedName;
        Description = normalizedDescription;
        Category = category;
        DurationMinutes = durationMinutes;
        MaxCapacity = maxCapacity;
        MarkAsUpdated();
    }

    /// <summary>
    /// Retira la clase del catálogo: no se pueden programar sesiones nuevas de ella.
    /// </summary>
    public void Deactivate()
    {
        if (Status == GymClassStatus.Inactive)
            throw new BusinessRuleException("La clase ya se encuentra inactiva.");

        Status = GymClassStatus.Inactive;
        MarkAsUpdated();
    }

    public void Activate()
    {
        if (Status == GymClassStatus.Active)
            throw new BusinessRuleException("La clase ya se encuentra activa.");

        Status = GymClassStatus.Active;
        MarkAsUpdated();
    }

    private void EnsureIsActive()
    {
        if (!IsActive)
            throw new BusinessRuleException("No se pueden modificar los datos de una clase inactiva.");
    }

    private void MarkAsUpdated() => UpdatedAt = DateTime.UtcNow;

    private static void ApplyNameRules(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleException("El nombre de la clase es requerido.");

        if (name.Length < NAME_MIN_LENGTH || name.Length > NAME_MAX_LENGTH)
            throw new BusinessRuleException(
                $"El nombre de la clase debe tener entre {NAME_MIN_LENGTH} y {NAME_MAX_LENGTH} caracteres.");
    }

    private static void ApplyDescriptionRules(string description)
    {
        if (description.Length > DESCRIPTION_MAX_LENGTH)
            throw new BusinessRuleException(
                $"La descripción no puede superar {DESCRIPTION_MAX_LENGTH} caracteres.");
    }

    private static void ApplyCategoryRules(ClassCategory category)
    {
        if (!Enum.IsDefined(category))
            throw new BusinessRuleException("La categoría de la clase no es válida.");
    }

    private static void ApplyDurationRules(int durationMinutes)
    {
        if (durationMinutes < MIN_DURATION_MINUTES || durationMinutes > MAX_DURATION_MINUTES)
            throw new BusinessRuleException(
                $"La duración debe estar entre {MIN_DURATION_MINUTES} y {MAX_DURATION_MINUTES} minutos.");
    }

    private static void ApplyCapacityRules(int maxCapacity)
    {
        if (maxCapacity < MIN_CAPACITY || maxCapacity > MAX_CAPACITY)
            throw new BusinessRuleException(
                $"El aforo máximo debe estar entre {MIN_CAPACITY} y {MAX_CAPACITY} personas.");
    }
}
