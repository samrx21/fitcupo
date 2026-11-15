using FitCupo.Classes.Application.Contracts.Persistence;
using FitCupo.Classes.Application.Contracts.Repositories;
using FitCupo.Classes.Application.Exceptions;
using FitCupo.Classes.Application.Utilities.Mediator;
using FitCupo.Classes.Domain.Entities.GymClasses;

namespace FitCupo.Classes.Application.UseCases.GymClasses.Commands.ActivateGymClass;

public sealed class ActivateGymClassUseCase(IGymClassRepository gymClassRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<ActivateGymClassCommand>
{
    public async Task Handle(ActivateGymClassCommand request, CancellationToken cancellationToken)
    {
        GymClass gymClass = await gymClassRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"No existe una clase con el Id {request.Id}.");

        gymClass.Activate();

        await gymClassRepository.UpdateAsync(gymClass, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
