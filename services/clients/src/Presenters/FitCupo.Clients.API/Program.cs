using System.Text.Json.Serialization;
using FitCupo.Clients.API.Middlewares;
using FitCupo.Clients.Application;
using FitCupo.Clients.Persistence;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
        // Los enums viajan como texto ("CitizenshipCard", "Active") en lugar de números
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Cada capa registra sus propias dependencias por medio de un método de extensión
builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);

WebApplication app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options.WithTitle("FitCupo · Clientes"));
    await app.Services.ApplyMigrationsAsync();
}

app.MapControllers();

await app.RunAsync();
