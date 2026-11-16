using FitCupo.Classes.Application.Contracts.Persistence;
using FitCupo.Classes.Application.Contracts.Repositories;
using FitCupo.Classes.Application.Exceptions;
using FitCupo.Classes.Application.Utilities.Mediator;
using FitCupo.Classes.Domain.Entities.ClassSessions;

namespace FitCupo.Classes.Application.UseCases.ClassSessions.Commands.CancelClassSession;

public sealed class CancelClassSessionUseCase(IClassSessionRepository classSessionRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CancelClassSessionCommand>
{
    public async Task Handle(CancelClassSessionCommand request, CancellationToken cancellationToken)
    {
        ClassSession session = await classSessionRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"No existe una sesión con el Id {request.Id}.");

        // Autorización por usuario: el rol permite cancelar, pero solo las sesiones propias
        if (!request.CanCancelAnySession && !session.IsTaughtBy(request.RequestedBy))
            throw new ForbiddenException("Solo puedes cancelar las sesiones que tú dictas.");

        session.Cancel(request.Reason);

        await classSessionRepository.UpdateAsync(session, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
    }
}
