using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kit.Exe.Environment;
using Kit.ExecutableKit.HealthCheck;
using Kit.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Router.Executable.Admin.Components;
using Router.Executable.Admin.Configuration;
using Router.Executable.Admin.Services;
using Router.Executable.Admin.Services.Validation;

var builder = WebApplication.CreateBuilder(args);

var environment = new EnvironmentVariableRuntimeEnvironment();
var configuration = environment.BuildConfiguration();

builder.Services.Configure<UserCredentialsConfig>(configuration.GetSection(nameof(UserCredentialsConfig)));
builder.Services.Configure<RouterApiConfiguration>(configuration.GetSection(nameof(RouterApiConfiguration)));

builder.WebHost.UseKestrel(options => options.Listen(IPAddress.Any, 5101));

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddConsoleLogging();

builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/login";
            options.ExpireTimeSpan = TimeSpan.FromDays(60);
            options.SlidingExpiration = true;
            options.Events = new CookieAuthenticationEvents
            {
                OnValidatePrincipal = async context =>
                {
                    var auth = context.HttpContext.RequestServices.GetRequiredService<AuthService>();
                    var storedHash = context.Principal?.FindFirstValue("CredentialsHash");

                    if (storedHash != auth.GetCredentialsHash())
                    {
                        context.RejectPrincipal();
                        await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    }
                }
            };
        });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<AuthService>();

builder.Services.Configure<JsonSerializerOptions>(options =>
        options.Converters.Add(new JsonStringEnumConverter()));

builder.Services
        .AddScoped<IRoutingService, RoutingService>()
        .AddScoped<ITargetsService, TargetsService>()
        .AddScoped<IRoutingValidator, RoutingValidator>()
        .AddScoped<IRuleValidator, RuleValidator>()
        .AddScoped<ITargetsValidator, TargetsValidator>()
        .AddScoped<IRoutingConfigurationValidator, RoutingConfigurationValidator>();

builder.Services.AddHttpClient<IRoutingConfigurationService, RoutingConfigurationService>((sp, client) =>
    {
        var options = sp.GetRequiredService<IOptions<RouterApiConfiguration>>();
        client.BaseAddress = new Uri(options.Value.BaseUrl);
        client.DefaultRequestHeaders.Add("Authorization", options.Value.AuthorizationToken);
    });

builder.Services.AddHealthChecks()
        .Add(new HealthCheckRegistration(
            "default",
            _ => new DefaultHealthCheck(),
            null,
            ["default"]));

var app = builder.Build();

app.UseExceptionHandler("/Error", createScopeForErrors: true);
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/account/login", async (HttpContext ctx, AuthService auth, [FromForm] string login, [FromForm] string password) =>
{
    if (!auth.ValidateCredentials(login, password))
        return Results.Redirect("/login?error=1");

    await ctx.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        auth.CreatePrincipal(login, auth.GetCredentialsHash()));

    return Results.Redirect("/");
});

app.MapPost("/account/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.UseHealthChecks("/health");

app.Run();