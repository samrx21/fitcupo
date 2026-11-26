using FitCupo.Web;
using FitCupo.Web.Auth;
using FitCupo.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// La interfaz solo conoce al API Gateway; él reparte cada petición al microservicio que corresponde
string gatewayUrl = builder.Configuration["GatewayUrl"]
    ?? throw new InvalidOperationException("Falta GatewayUrl en wwwroot/appsettings.json.");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(gatewayUrl) });

// Sesión del usuario a partir del JWT que emite Identity
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<TokenStorage>();
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(services =>
    services.GetRequiredService<JwtAuthenticationStateProvider>());

builder.Services.AddScoped<ApiClient>();
builder.Services.AddScoped<AuthService>();

await builder.Build().RunAsync();
