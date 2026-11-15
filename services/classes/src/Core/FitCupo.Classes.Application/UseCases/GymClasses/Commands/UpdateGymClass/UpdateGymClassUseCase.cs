using FitCupo.Classes.Application.Contracts.Persistence;
using FitCupo.Classes.Application.Contracts.Repositories;
using FitCupo.Classes.Application.Exceptions;
using FitCupo.Classes.Application.Utilities.Mediator;
using FitCupo.Classes.Domain.Entities.GymClasses;

namespace FitCupo.Classes.Application.UseCases.GymClasses.Commands.UpdateGymClass;

public sealed class UpdateGymClassUseCase(IGymClassRepository gymClassRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateGymClassCommand>
{
    public async Task Handle(UpdateGymClassCommand request, CancellationToken cancellationToken)
    {
        GymClass gymClass = await gymClassRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"No existe una clase con el Id {request.Id}.");

        gymClass.Update(
            request.Name,
            request.Description,
            request.Category,
            request.DurationMinutes,
            request.MaxCapacity);

        if (await gymClassRepository.ExistsByNameAsync(gymClass.Name, gymClass.Id, cancellationToken))
            throw new ConflictException($"Ya existe otra clase con el nombre {gymClass.Name}.");

        await gymClassRepository.UpdateAsync(gymClass, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
