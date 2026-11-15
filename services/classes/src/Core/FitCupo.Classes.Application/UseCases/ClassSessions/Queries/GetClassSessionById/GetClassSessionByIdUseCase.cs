using FitCupo.Classes.Application.Contracts.Repositories;
using FitCupo.Classes.Application.Exceptions;
using FitCupo.Classes.Application.Utilities.Mediator;

namespace FitCupo.Classes.Application.UseCases.ClassSessions.Queries.GetClassSessionById;

public sealed class GetClassSessionByIdUseCase(IClassSessionRepository classSessionRepository)
    : IRequestHandler<GetClassSessionByIdQuery, ClassSessionDto>
{
    public async Task<ClassSessionDto> Handle(GetClassSessionByIdQuery request, CancellationToken cancellationToken) =>
        await classSessionRepository.GetDtoByIdAsync(request.Id, cancellationToken)
        ?? throw new NotFoundException($"No existe una sesión con el Id {request.Id}.");
}
