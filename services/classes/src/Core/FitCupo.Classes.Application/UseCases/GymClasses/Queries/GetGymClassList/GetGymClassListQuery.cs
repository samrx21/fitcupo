using FitCupo.Classes.Application.Utilities.Mediator;
using FitCupo.Classes.Application.Utilities.Pagination;
using FitCupo.Classes.Domain.Entities.GymClasses;

namespace FitCupo.Classes.Application.UseCases.GymClasses.Queries.GetGymClassList;

/// <summary>
/// Query para listar el catálogo de clases de forma paginada, con búsqueda por nombre
/// y filtros por categoría y estado.
/// </summary>
public sealed record GetGymClassListQuery : IRequest<PaginationResponse<GymClassDto>>
{
    public PaginationRequest Pagination { get; init; } = PaginationRequest.Standard();
    public string? Search { get; init; }
    public ClassCategory? Category { get; init; }
    public GymClassStatus? Status { get; init; }
}
