using FitCupo.Bookings.Domain.Entities.Bookings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FitCupo.Bookings.Persistence.Configurations;

/// <summary>
/// Mapeo de la entidad Booking a la tabla Bookings. La sesión y el cliente viven en otros
/// microservicios, así que no hay llaves foráneas: solo sus Id y una copia de sus datos.
/// </summary>
internal sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");
        builder.HasKey(booking => booking.Id);
        builder.Property(booking => booking.Id).ValueGeneratedNever();

        builder.Property(booking => booking.SessionId).IsRequired();
        builder.Property(booking => booking.GymClassName).HasMaxLength(Booking.CLASS_NAME_MAX_LENGTH).IsRequired();
        builder.Property(booking => booking.Room).HasMaxLength(Booking.ROOM_MAX_LENGTH).IsRequired();
        builder.Property(booking => booking.InstructorId).IsRequired();
        builder.Property(booking => booking.InstructorName).HasMaxLength(Booking.INSTRUCTOR_NAME_MAX_LENGTH).IsRequired();
        builder.Property(booking => booking.StartsAt).HasConversion(ToUtc).IsRequired();
        builder.Property(booking => booking.EndsAt).HasConversion(ToUtc).IsRequired();

        builder.Property(booking => booking.ClientId).IsRequired();
        builder.Property(booking => booking.ClientName).HasMaxLength(Booking.CLIENT_NAME_MAX_LENGTH).IsRequired();
        builder.Property(booking => booking.ClientEmail).HasMaxLength(Booking.CLIENT_EMAIL_MAX_LENGTH).IsRequired();

        builder.Property(booking => booking.Status).HasConversion<string>().HasMaxLength(16).IsRequired();
        builder.Property(booking => booking.BookedAt).HasConversion(ToUtc).IsRequired();
        builder.Property(booking => booking.CancelledAt)
            .HasConversion(
                value => value,
                value => value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : value);

        // Un cliente no puede tener dos reservas confirmadas en la misma sesión.
        // El índice filtrado lo garantiza incluso si llegan dos peticiones al mismo tiempo.
        builder.HasIndex(booking => new { booking.SessionId, booking.ClientId })
            .IsUnique()
            .HasFilter($"[Status] = '{BookingStatus.Confirmed}'")
            .HasDatabaseName("UX_Bookings_Session_Client_Confirmed");

        builder.HasIndex(booking => new { booking.SessionId, booking.Status });
        builder.HasIndex(booking => booking.ClientEmail);
        builder.HasIndex(booking => booking.StartsAt);
    }

    // SQL Server no guarda la zona horaria: al leer, se marcan las fechas como UTC
    private static readonly ValueConverter<DateTime, DateTime> ToUtc =
        new(value => value, value => DateTime.SpecifyKind(value, DateTimeKind.Utc));
}
