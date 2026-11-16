using FitCupo.Identity.Application.Models;

namespace FitCupo.Identity.Application.UseCases.Users;

internal static class UserMapperExtensions
{
    public static UserDto ToDto(this UserAccount user) =>
        new()
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Roles = user.Roles
        };
}
