using FitCupo.Clients.Domain.Common.ValueObjects;
using FitCupo.Clients.Domain.Entities.Clients;
using FitCupo.Clients.Domain.Entities.Clients.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitCupo.Clients.Persistence.Configurations;

/// <summary>
/// Mapeo de la entidad Client a la tabla Clients. Los value objects se guardan
/// como columnas de la misma tabla (owned types).
/// </summary>
internal sealed class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Clients");
        builder.HasKey(client => client.Id);
        builder.Property(client => client.Id).ValueGeneratedNever();

        builder.Property(client => client.FirstName).HasMaxLength(Client.NAME_MAX_LENGTH).IsRequired();
        builder.Property(client => client.LastName).HasMaxLength(Client.NAME_MAX_LENGTH).IsRequired();
        builder.Property(client => client.BirthDate).IsRequired();
        builder.Property(client => client.Status).HasConversion<string>().HasMaxLength(16).IsRequired();
        // SQL Server no guarda la zona horaria: al leer, se marcan las fechas como UTC
        builder.Property(client => client.CreatedAt)
            .HasConversion(value => value, value => DateTime.SpecifyKind(value, DateTimeKind.Utc))
            .IsRequired();
        builder.Property(client => client.UpdatedAt)
            .HasConversion(
                value => value,
                value => value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : value);
        builder.Ignore(client => client.FullName);

        builder.OwnsOne(client => client.Document, document =>
        {
            document.Property(value => value.Type)
                .HasColumnName("DocumentType")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();
            document.Property(value => value.Number)
                .HasColumnName("DocumentNumber")
                .HasMaxLength(Document.MAX_LENGTH)
                .IsRequired();
            document.HasIndex(value => new { value.Type, value.Number }).IsUnique();
        });
        builder.Navigation(client => client.Document).IsRequired();

        builder.OwnsOne(client => client.Email, email =>
        {
            email.Property(value => value.Value)
                .HasColumnName("Email")
                .HasMaxLength(Email.MAX_LENGTH)
                .IsRequired();
            email.HasIndex(value => value.Value).IsUnique();
        });
        builder.Navigation(client => client.Email).IsRequired();

        builder.OwnsOne(client => client.Phone, phone =>
        {
            phone.Property(value => value.Value)
                .HasColumnName("Phone")
                .HasMaxLength(PhoneNumber.MAX_DIGITS + 1)
                .IsRequired();
        });
        builder.Navigation(client => client.Phone).IsRequired();

        builder.OwnsOne(client => client.EmergencyContact, contact =>
        {
            contact.Property(value => value.Name)
                .HasColumnName("EmergencyContactName")
                .HasMaxLength(EmergencyContact.NAME_MAX_LENGTH)
                .IsRequired();
            contact.OwnsOne(value => value.Phone, phone =>
            {
                phone.Property(value => value.Value)
                    .HasColumnName("EmergencyContactPhone")
                    .HasMaxLength(PhoneNumber.MAX_DIGITS + 1)
                    .IsRequired();
            });
            contact.Navigation(value => value.Phone).IsRequired();
        });
        builder.Navigation(client => client.EmergencyContact).IsRequired();
    }
}
