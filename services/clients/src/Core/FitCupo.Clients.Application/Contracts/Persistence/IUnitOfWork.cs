namespace FitCupo.Clients.Application.Contracts.Persistence;

/// <summary>
/// Confirma en un solo bloque todos los cambios que los repositorios prepararon
/// durante un comando, de modo que se guardan todos o ninguno.
/// </summary>
public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken cancellationToken = default);

    Task RollbackAsync(CancellationToken cancellationToken = default);
}
