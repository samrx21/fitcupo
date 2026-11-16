using FitCupo.Classes.Domain.Entities.GymClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitCupo.Classes.Persistence.Configurations;

/// <summary>
/// Mapeo de la entidad GymClass a la tabla GymClasses, con tres clases iniciales.
/// </summary>
internal sealed class GymClassConfiguration : IEntityTypeConfiguration<GymClass>
{
    private static readonly DateTime SeedDate = new(2026, 11, 1, 0, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<GymClass> builder)
    {
        builder.ToTable("GymClasses");
        builder.HasKey(gymClass => gymClass.Id);
        builder.Property(gymClass => gymClass.Id).ValueGeneratedNever();

        builder.Property(gymClass => gymClass.Name).HasMaxLength(GymClass.NAME_MAX_LENGTH).IsRequired();
        builder.HasIndex(gymClass => gymClass.Name).IsUnique();
        builder.Property(gymClass => gymClass.Description).HasMaxLength(GymClass.DESCRIPTION_MAX_LENGTH).IsRequired();
        builder.Property(gymClass => gymClass.Category).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(gymClass => gymClass.DurationMinutes).IsRequired();
        builder.Property(gymClass => gymClass.MaxCapacity).IsRequired();
        builder.Property(gymClass => gymClass.Status).HasConversion<string>().HasMaxLength(16).IsRequired();
        // SQL Server no guarda la zona horaria: al leer, se marcan las fechas como UTC
        builder.Property(gymClass => gymClass.CreatedAt)
            .HasConversion(value => value, value => DateTime.SpecifyKind(value, DateTimeKind.Utc))
            .IsRequired();
        builder.Property(gymClass => gymClass.UpdatedAt)
            .HasConversion(
                value => value,
                value => value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : value);
        builder.Ignore(gymClass => gymClass.IsActive);

        // Datos iniciales: se crean con la migración para tener clases desde el primer arranque
        builder.HasData(
            SeedClass("019a4a00-0000-7000-8000-000000000001", "Spinning",
                "Ciclismo bajo techo con intervalos de alta intensidad.", ClassCategory.Cycling, 45, 20),
            SeedClass("019a4a00-0000-7000-8000-000000000002", "Yoga",
                "Posturas, respiración y flexibilidad para todos los niveles.", ClassCategory.Yoga, 60, 15),
            SeedClass("019a4a00-0000-7000-8000-000000000003", "Entrenamiento funcional",
                "Circuitos con peso corporal, kettlebells y TRX.", ClassCategory.Functional, 50, 12));
    }

    private static object SeedClass(
        string id, string name, string description, ClassCategory category, int durationMinutes, int maxCapacity) =>
        new
        {
            Id = Guid.Parse(id),
            Name = name,
            Description = description,
            Category = category,
            DurationMinutes = durationMinutes,
            MaxCapacity = maxCapacity,
            Status = GymClassStatus.Active,
            CreatedAt = SeedDate,
            UpdatedAt = (DateTime?)null
        };
}
