using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FitCupo.Identity.API.Security;

/// <summary>
/// Configura la validación de los JWT que emite Identity. Se valida la firma, el emisor,
/// la audiencia y la vigencia; no basta con decodificar el token.
/// </summary>
public static class JwtAuthenticationExtensions
{
    public const string SECTION_NAME = "Jwt";

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        JwtValidationSettings settings = configuration.GetSection(SECTION_NAME).Get<JwtValidationSettings>()
            ?? throw new InvalidOperationException($"La sección '{SECTION_NAME}' no está configurada.");
        settings.EnsureIsValid();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // Conserva los nombres cortos de los claims ("sub", "role") tal como los emite Identity
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = settings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = settings.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SigningKey)),
                    ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                    RequireSignedTokens = true,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    // Sin tolerancia: el token deja de servir en el segundo exacto en que expira
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = TokenClaims.NAME,
                    RoleClaimType = TokenClaims.ROLE
                };
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = WriteUnauthorizedAsync,
                    OnForbidden = WriteForbiddenAsync
                };
            });

        services.AddAuthorization();

        return services;
    }

    /// <summary>
    /// 401: la petición no trae un token válido (falta, está mal formado, fue modificado o expiró).
    /// </summary>
    private static async Task WriteUnauthorizedAsync(JwtBearerChallengeContext context)
    {
        // Reemplaza la respuesta vacía por defecto por una con formato ProblemDetails
        context.HandleResponse();

        string detail = context.AuthenticateFailure switch
        {
            null => "Debes enviar un token JWT en el encabezado Authorization: Bearer {token}.",
            SecurityTokenExpiredException expired =>
                $"El token expiró el {expired.Expires:yyyy-MM-dd HH:mm:ss} UTC. Inicia sesión de nuevo.",
            SecurityTokenInvalidSignatureException or SecurityTokenSignatureKeyNotFoundException =>
                "La firma del token no es válida: el token fue modificado o no lo emitió Identity.",
            SecurityTokenInvalidIssuerException => "El token no fue emitido por FitCupo Identity.",
            SecurityTokenInvalidAudienceException => "El token no está dirigido a los servicios de FitCupo.",
            _ => "El token no es válido."
        };

        context.Response.Headers.WWWAuthenticate = context.Error is null
            ? JwtBearerDefaults.AuthenticationScheme
            : $"{JwtBearerDefaults.AuthenticationScheme} error=\"{context.Error}\", error_description=\"{context.ErrorDescription}\"";

        await WriteProblemAsync(context.HttpContext, StatusCodes.Status401Unauthorized, "No autenticado", detail);
    }

    /// <summary>
    /// 403: el token es válido, pero el usuario no tiene el rol que exige el endpoint.
    /// </summary>
    private static Task WriteForbiddenAsync(ForbiddenContext context)
    {
        ClaimsPrincipal principal = context.HttpContext.User;
        string user = principal.Identity?.Name ?? "El usuario";
        string roles = string.Join(", ", principal.FindAll(TokenClaims.ROLE).Select(claim => claim.Value));

        return WriteProblemAsync(
            context.HttpContext,
            StatusCodes.Status403Forbidden,
            "Acceso denegado",
            $"{user} inició sesión, pero su rol ({roles}) no tiene permiso para este recurso.");
    }

    private static async Task WriteProblemAsync(HttpContext httpContext, int statusCode, string title, string detail)
    {
        httpContext.Response.StatusCode = statusCode;

        IProblemDetailsService problemDetailsService = httpContext.RequestServices.GetRequiredService<IProblemDetailsService>();
        await problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails { Status = statusCode, Title = title, Detail = detail }
        });
    }
}
