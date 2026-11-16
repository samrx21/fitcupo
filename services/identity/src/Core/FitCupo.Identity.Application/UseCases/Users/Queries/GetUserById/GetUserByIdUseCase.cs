using FitCupo.Identity.Application.Contracts.Identity;
using FitCupo.Identity.Application.Exceptions;
using FitCupo.Identity.Application.Models;
using FitCupo.Identity.Application.Utilities.Mediator;

namespace FitCupo.Identity.Application.UseCases.Users.Queries.GetUserById;

public sealed class GetUserByIdUseCase(IIdentityService identityService)
    : IRequestHandler<GetUserByIdQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        UserAccount user = await identityService.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"No existe un usuario con el Id {request.Id}.");

        return user.ToDto();
    }
}
