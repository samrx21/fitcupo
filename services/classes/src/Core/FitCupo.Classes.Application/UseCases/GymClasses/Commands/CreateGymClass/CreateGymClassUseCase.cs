using FitCupo.Classes.Application.Contracts.Persistence;
using FitCupo.Classes.Application.Contracts.Repositories;
using FitCupo.Classes.Application.Exceptions;
using FitCupo.Classes.Application.Utilities.Mediator;
using FitCupo.Classes.Domain.Entities.GymClasses;

namespace FitCupo.Classes.Application.UseCases.GymClasses.Commands.CreateGymClass;

public sealed class CreateGymClassUseCase(IGymClassRepository gymClassRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateGymClassCommand, Guid>
{
    public async Task<Guid> Handle(CreateGymClassCommand request, CancellationToken cancellationToken)
    {
        GymClass gymClass = new(
            request.Name,
            request.Description,
            request.Category,
            request.DurationMinutes,
            request.MaxCapacity);

        // La unicidad depende de los datos guardados, por eso se valida aquí y no en el dominio
        if (await gymClassRepository.ExistsByNameAsync(gymClass.Name, cancellationToken: cancellationToken))
            throw new ConflictException($"Ya existe una clase con el nombre {gymClass.Name}.");

        await gymClassRepository.AddAsync(gymClass, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return gymClass.Id;
    }
}
