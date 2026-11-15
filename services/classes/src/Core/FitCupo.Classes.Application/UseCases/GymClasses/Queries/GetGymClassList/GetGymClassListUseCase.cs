using FitCupo.Classes.Application.Contracts.Repositories;
using FitCupo.Classes.Application.Utilities.Mediator;
using FitCupo.Classes.Application.Utilities.Pagination;
using FitCupo.Classes.Domain.Entities.GymClasses;

namespace FitCupo.Classes.Application.UseCases.GymClasses.Queries.GetGymClassList;

public sealed class GetGymClassListUseCase(IGymClassRepository gymClassRepository)
    : IRequestHandler<GetGymClassListQuery, PaginationResponse<GymClassDto>>
{
    public async Task<PaginationResponse<GymClassDto>> Handle(
        GetGymClassListQuery request, CancellationToken cancellationToken)
    {
        PaginationResponse<GymClass> page = await gymClassRepository.GetPagedListAsync(
            request.Pagination, request.Search, request.Category, request.Status, cancellationToken);

        List<GymClassDto> items = page.Items.Select(gymClass => gymClass.ToDto()).ToList();

        return PaginationResponse<GymClassDto>.Create(items, page.TotalCount, request.Pagination);
    }
}
