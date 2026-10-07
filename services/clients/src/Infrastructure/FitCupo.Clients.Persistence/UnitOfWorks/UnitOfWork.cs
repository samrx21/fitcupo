using FitCupo.Clients.Application.Contracts.Persistence;

namespace FitCupo.Clients.Persistence.UnitOfWorks;

/// <summary>
/// Implementa el Unit of Work sobre el DbContext de EF Core, que ya agrupa
/// todos los cambios pendientes y los guarda en una sola transacción.
/// </summary>
public sealed class UnitOfWork(ClientsDbContext context) : IUnitOfWork
{
    public async Task CommitAsync(CancellationToken cancellationToken = default) =>
        await context.SaveChangesAsync(cancellationToken);

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        // Descarta los cambios que se prepararon y todavía no se han guardado
        context.ChangeTracker.Clear();
        return Task.CompletedTask;
    }
}
