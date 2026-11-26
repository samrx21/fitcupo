using FitCupo.Gateway.ServiceStatus;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

const string WEB_CORS_POLICY = "FitCupoWeb";

// La interfaz web corre en otro origen (puerto 5300): se le permite llamar al Gateway
string[] allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options => options.AddPolicy(WEB_CORS_POLICY, policy => policy
    .WithOrigins(allowedOrigins)
    .AllowAnyHeader()
    .AllowAnyMethod()));

// YARP lee las rutas y los microservicios de destino de la sección "ReverseProxy" de appsettings.
// El encabezado Authorization (JWT) pasa sin cambios: cada microservicio lo valida por su cuenta.
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddHttpClient(ServiceStatusEndpoint.HTTP_CLIENT_NAME);

WebApplication app = builder.Build();

app.UseCors(WEB_CORS_POLICY);

app.MapGet("/", () => Results.Ok(new
{
    name = "FitCupo · API Gateway",
    routes = new[] { "/api/auth", "/api/users", "/api/clients", "/api/classes", "/api/sessions", "/api/bookings", "/api/status" }
}));

app.MapServiceStatus();
app.MapReverseProxy();

await app.RunAsync();
