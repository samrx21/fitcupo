using FitCupo.Classes.Application.Contracts.Persistence;
using FitCupo.Classes.Application.Contracts.Repositories;
using FitCupo.Classes.Application.Exceptions;
using FitCupo.Classes.Application.Utilities.Mediator;
using FitCupo.Classes.Domain.Entities.GymClasses;

namespace FitCupo.Classes.Application.UseCases.GymClasses.Commands.DeactivateGymClass;

public sealed class DeactivateGymClassUseCase(IGymClassRepository gymClassRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeactivateGymClassCommand>
{
    public async Task Handle(DeactivateGymClassCommand request, CancellationToken cancellationToken)
    {
        GymClass gymClass = await gymClassRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"No existe una clase con el Id {request.Id}.");

        gymClass.Deactivate();

        await gymClassRepository.UpdateAsync(gymClass, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
