using FitCupo.Identity.Application.Contracts.Identity;
using FitCupo.Identity.Application.Exceptions;
using FitCupo.Identity.Application.Models;
using FitCupo.Identity.Application.UseCases.Users;
using FitCupo.Identity.Application.Utilities.Mediator;

namespace FitCupo.Identity.Application.UseCases.Auth.Commands.Login;

public sealed class LoginUseCase(IIdentityService identityService, ITokenGenerator tokenGenerator)
    : IRequestHandler<LoginCommand, LoginResponseDto>
{
    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            throw new ValidationException("El correo y la contraseña son requeridos.");

        // Identity compara la contraseña con su hash y controla el bloqueo por intentos fallidos
        UserAccount user = await identityService.AuthenticateAsync(
            request.Email.Trim(), request.Password, cancellationToken);

        AccessToken token = tokenGenerator.Generate(user);

        return new LoginResponseDto
        {
            AccessToken = token.Token,
            ExpiresAt = token.ExpiresAt,
            User = user.ToDto()
        };
    }
}
