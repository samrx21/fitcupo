using FitCupo.Identity.API.Middlewares;
using FitCupo.Identity.API.OpenApi;
using FitCupo.Identity.API.Security;
using FitCupo.Identity.Application;
using FitCupo.Identity.Infrastructure;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Identity también valida sus propios tokens para proteger /api/auth/me y /api/users
builder.Services.AddJwtAuthentication(builder.Configuration);

// Cada capa registra sus propias dependencias por medio de un método de extensión
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

WebApplication app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options.WithTitle("FitCupo · Identity"));
    // Crea la base de datos y ejecuta los seeders de roles y usuarios
    await app.Services.InitializeDatabaseAsync();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
