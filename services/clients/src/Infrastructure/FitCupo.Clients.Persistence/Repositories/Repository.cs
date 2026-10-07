using FitCupo.Clients.Application.Contracts.Repositories;

namespace FitCupo.Clients.Persistence.Repositories;

/// <summary>
/// Implementación genérica de las operaciones básicas con EF Core.
/// Las escrituras solo quedan preparadas en el contexto; el UnitOfWork las confirma.
/// </summary>
public class Repository<TEntity>(ClientsDbContext context) : IRepository<TEntity> where TEntity : class
{
    protected ClientsDbContext Context { get; } = context;

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await Context.Set<TEntity>().AddAsync(entity, cancellationToken);
        return entity;
    }

    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Context.Set<TEntity>().Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Context.Set<TEntity>().Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await Context.Set<TEntity>().FindAsync([id], cancellationToken);
}
