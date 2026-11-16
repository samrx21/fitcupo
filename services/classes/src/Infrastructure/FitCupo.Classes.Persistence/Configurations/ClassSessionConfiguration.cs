using FitCupo.Classes.Domain.Entities.ClassSessions;
using FitCupo.Classes.Domain.Entities.GymClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitCupo.Classes.Persistence.Configurations;

/// <summary>
/// Mapeo de la entidad ClassSession a la tabla ClassSessions. La franja horaria
/// (value object TimeSlot) se guarda en las columnas StartsAt y EndsAt.
/// </summary>
internal sealed class ClassSessionConfiguration : IEntityTypeConfiguration<ClassSession>
{
    public void Configure(EntityTypeBuilder<ClassSession> builder)
    {
        builder.ToTable("ClassSessions");
        builder.HasKey(session => session.Id);
        builder.Property(session => session.Id).ValueGeneratedNever();

        // Cada sesión pertenece a una clase; no se puede borrar una clase con sesiones
        builder.HasOne<GymClass>()
            .WithMany()
            .HasForeignKey(session => session.GymClassId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(session => session.InstructorId).IsRequired();
        builder.HasIndex(session => session.InstructorId);
        builder.Property(session => session.InstructorName)
            .HasMaxLength(ClassSession.INSTRUCTOR_NAME_MAX_LENGTH)
            .IsRequired();
        builder.Property(session => session.Room).HasMaxLength(ClassSession.ROOM_MAX_LENGTH).IsRequired();
        builder.Property(session => session.Capacity).IsRequired();
        builder.Property(session => session.Status).HasConversion<string>().HasMaxLength(16).IsRequired();
        builder.Property(session => session.CancellationReason).HasMaxLength(ClassSession.REASON_MAX_LENGTH);
        // SQL Server no guarda la zona horaria: al leer, se marcan las fechas como UTC
        builder.Property(session => session.CreatedAt)
            .HasConversion(value => value, value => DateTime.SpecifyKind(value, DateTimeKind.Utc))
            .IsRequired();
        builder.Property(session => session.CancelledAt)
            .HasConversion(
                value => value,
                value => value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : value);

        builder.OwnsOne(session => session.TimeSlot, schedule =>
        {
            schedule.Property(value => value.StartsAt)
                .HasColumnName("StartsAt")
                .HasConversion(value => value, value => DateTime.SpecifyKind(value, DateTimeKind.Utc))
                .IsRequired();
            schedule.Property(value => value.EndsAt)
                .HasColumnName("EndsAt")
                .HasConversion(value => value, value => DateTime.SpecifyKind(value, DateTimeKind.Utc))
                .IsRequired();
            schedule.HasIndex(value => value.StartsAt);
        });
        builder.Navigation(session => session.TimeSlot).IsRequired();
    }
}
