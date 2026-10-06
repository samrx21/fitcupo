namespace FitCupo.Clients.Application.Contracts.Repositories;

/// <summary>
/// Operaciones básicas que comparten todos los repositorios.
/// Las escrituras solo se preparan aquí; las confirma el IUnitOfWork.
/// </summary>
public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
