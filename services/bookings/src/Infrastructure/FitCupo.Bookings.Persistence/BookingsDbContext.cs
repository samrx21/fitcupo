using FitCupo.Bookings.Domain.Entities.Bookings;
using Microsoft.EntityFrameworkCore;

namespace FitCupo.Bookings.Persistence;

public sealed class BookingsDbContext(DbContextOptions<BookingsDbContext> options) : DbContext(options)
{
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Carga todas las clases IEntityTypeConfiguration de este proyecto (carpeta Configurations)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookingsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
