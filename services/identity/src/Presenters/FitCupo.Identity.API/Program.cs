using FitCupo.Identity.Application;
using FitCupo.Identity.Infrastructure;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

// Cada capa registra sus propias dependencias por medio de un método de extensión
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options.WithTitle("FitCupo · Identity"));
    // Crea la base de datos y ejecuta los seeders de roles y usuarios
    await app.Services.InitializeDatabaseAsync();
}

app.MapControllers();

await app.RunAsync();
