using FitCupo.Classes.Application.Contracts.Repositories;
using FitCupo.Classes.Application.Utilities.Pagination;
using FitCupo.Classes.Domain.Entities.GymClasses;
using FitCupo.Classes.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FitCupo.Classes.Persistence.Repositories;

public sealed class GymClassRepository(ClassesDbContext context) : Repository<GymClass>(context), IGymClassRepository
{
    public async Task<bool> ExistsByNameAsync(
        string name, Guid? excludedGymClassId = null, CancellationToken cancellationToken = default) =>
        await Context.GymClasses.AnyAsync(
            gymClass => gymClass.Name == name && gymClass.Id != excludedGymClassId,
            cancellationToken);

    public async Task<PaginationResponse<GymClass>> GetPagedListAsync(
        PaginationRequest pagination,
        string? search,
        ClassCategory? category,
        GymClassStatus? status,
        CancellationToken cancellationToken = default)
    {
        IQueryable<GymClass> query = Context.GymClasses.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string term = search.Trim();
            query = query.Where(gymClass => gymClass.Name.Contains(term) || gymClass.Description.Contains(term));
        }

        if (category.HasValue)
            query = query.Where(gymClass => gymClass.Category == category.Value);

        if (status.HasValue)
            query = query.Where(gymClass => gymClass.Status == status.Value);

        query = query.OrderBy(gymClass => gymClass.Name).ThenBy(gymClass => gymClass.Id);

        return await query.ToPagedListAsync(pagination, cancellationToken);
    }
}
