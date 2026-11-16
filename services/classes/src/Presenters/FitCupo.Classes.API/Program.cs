using System.Text.Json.Serialization;
using FitCupo.Classes.API.Middlewares;
using FitCupo.Classes.API.OpenApi;
using FitCupo.Classes.API.Security;
using FitCupo.Classes.Application;
using FitCupo.Classes.Persistence;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
        // Los enums viajan como texto ("Yoga", "Scheduled") en lugar de números
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Valida los JWT que emite Identity: firma, emisor, audiencia y vigencia
builder.Services.AddJwtAuthentication(builder.Configuration);

// Cada capa registra sus propias dependencias por medio de un método de extensión
builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);

WebApplication app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options.WithTitle("FitCupo · Clases"));
    await app.Services.ApplyMigrationsAsync();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
