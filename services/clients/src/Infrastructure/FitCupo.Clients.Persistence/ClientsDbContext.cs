using FitCupo.Clients.Domain.Entities.Clients;
using Microsoft.EntityFrameworkCore;

namespace FitCupo.Clients.Persistence;

public sealed class ClientsDbContext(DbContextOptions<ClientsDbContext> options) : DbContext(options)
{
    public DbSet<Client> Clients => Set<Client>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Carga todas las clases IEntityTypeConfiguration de este proyecto (carpeta Configurations)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClientsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
