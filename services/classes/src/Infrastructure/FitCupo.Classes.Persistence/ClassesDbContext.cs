using FitCupo.Classes.Domain.Entities.ClassSessions;
using FitCupo.Classes.Domain.Entities.GymClasses;
using Microsoft.EntityFrameworkCore;

namespace FitCupo.Classes.Persistence;

public sealed class ClassesDbContext(DbContextOptions<ClassesDbContext> options) : DbContext(options)
{
    public DbSet<GymClass> GymClasses => Set<GymClass>();
    public DbSet<ClassSession> ClassSessions => Set<ClassSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Carga todas las clases IEntityTypeConfiguration de este proyecto (carpeta Configurations)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClassesDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
