using FitCupo.Classes.Application.Contracts.Repositories;
using FitCupo.Classes.Application.Exceptions;
using FitCupo.Classes.Application.Utilities.Mediator;
using FitCupo.Classes.Domain.Entities.GymClasses;

namespace FitCupo.Classes.Application.UseCases.GymClasses.Queries.GetGymClassById;

public sealed class GetGymClassByIdUseCase(IGymClassRepository gymClassRepository)
    : IRequestHandler<GetGymClassByIdQuery, GymClassDto>
{
    public async Task<GymClassDto> Handle(GetGymClassByIdQuery request, CancellationToken cancellationToken)
    {
        GymClass gymClass = await gymClassRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"No existe una clase con el Id {request.Id}.");

        return gymClass.ToDto();
    }
}
