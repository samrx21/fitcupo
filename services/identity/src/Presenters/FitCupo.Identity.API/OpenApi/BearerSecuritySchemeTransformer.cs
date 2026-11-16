using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace FitCupo.Identity.API.OpenApi;

/// <summary>
/// Declara en el documento OpenAPI que la API usa JWT (Bearer), para que Scalar
/// muestre el campo donde se pega el token.
/// </summary>
internal sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    public const string SCHEME_NAME = "Bearer";

    public Task TransformAsync(
        OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes[SCHEME_NAME] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "JWT emitido por FitCupo Identity en POST /api/auth/login."
        };

        document.Security ??= [];
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(SCHEME_NAME, document)] = []
        });

        return Task.CompletedTask;
    }
}
