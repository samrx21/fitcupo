using FitCupo.Classes.Application.Utilities.Pagination;
using FitCupo.Classes.Domain.Entities.GymClasses;

namespace FitCupo.Classes.Application.Contracts.Repositories;

/// <summary>
/// Operaciones propias del repositorio de clases.
/// </summary>
public interface IGymClassRepository : IRepository<GymClass>
{
    /// <summary>
    /// Indica si el nombre ya pertenece a otra clase. <paramref name="excludedGymClassId"/>
    /// permite ignorar a la propia clase cuando se actualizan sus datos.
    /// </summary>
    Task<bool> ExistsByNameAsync(
        string name, Guid? excludedGymClassId = null, CancellationToken cancellationToken = default);

    Task<PaginationResponse<GymClass>> GetPagedListAsync(
        PaginationRequest pagination,
        string? search,
        ClassCategory? category,
        GymClassStatus? status,
        CancellationToken cancellationToken = default);
}
