using FitCupo.Identity.Application.Contracts.Identity;
using FitCupo.Identity.Application.Models;
using FitCupo.Identity.Application.Utilities.Mediator;

namespace FitCupo.Identity.Application.UseCases.Users.Queries.GetUserList;

public sealed class GetUserListUseCase(IIdentityService identityService)
    : IRequestHandler<GetUserListQuery, IReadOnlyList<UserDto>>
{
    public async Task<IReadOnlyList<UserDto>> Handle(GetUserListQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<UserAccount> users = await identityService.GetListAsync(request.Role, cancellationToken);

        return users.Select(user => user.ToDto()).ToList();
    }
}
