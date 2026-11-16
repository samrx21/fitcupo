using FitCupo.Identity.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitCupo.Identity.Infrastructure.Persistence.Configurations;

internal sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("Users");

        builder.Property(user => user.FirstName).HasMaxLength(ApplicationUser.NAME_MAX_LENGTH).IsRequired();
        builder.Property(user => user.LastName).HasMaxLength(ApplicationUser.NAME_MAX_LENGTH).IsRequired();
        // SQL Server no guarda la zona horaria: al leer, se marca la fecha como UTC
        builder.Property(user => user.CreatedAt)
            .HasConversion(value => value, value => DateTime.SpecifyKind(value, DateTimeKind.Utc))
            .IsRequired();
    }
}
