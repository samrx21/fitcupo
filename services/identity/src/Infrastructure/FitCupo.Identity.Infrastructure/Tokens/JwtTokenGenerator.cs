using System.Security.Claims;
using System.Text;
using FitCupo.Identity.Application.Contracts.Identity;
using FitCupo.Identity.Application.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace FitCupo.Identity.Infrastructure.Tokens;

/// <summary>
/// Genera un JWT firmado con HMAC-SHA256. Lleva el Id, el correo, el nombre y los roles
/// del usuario, el emisor, la audiencia y la fecha de expiración.
/// </summary>
public sealed class JwtTokenGenerator(IOptions<JwtOptions> options) : ITokenGenerator
{
    // Nombres cortos de los claims; los microservicios leen los roles del claim "role"
    public const string ROLE_CLAIM = "role";

    private readonly JwtOptions _options = options.Value;

    public AccessToken Generate(UserAccount user)
    {
        DateTime issuedAt = DateTime.UtcNow;
        DateTime expiresAt = issuedAt.AddMinutes(_options.ExpirationMinutes);

        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Name, user.FullName),
            new(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new(JwtRegisteredClaimNames.FamilyName, user.LastName),
            // Identificador único del token
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];
        claims.AddRange(user.Roles.Select(role => new Claim(ROLE_CLAIM, role)));

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_options.SigningKey));

        SecurityTokenDescriptor descriptor = new()
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Subject = new ClaimsIdentity(claims),
            IssuedAt = issuedAt,
            NotBefore = issuedAt,
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        };

        string token = new JsonWebTokenHandler().CreateToken(descriptor);

        return new AccessToken(token, expiresAt);
    }
}
