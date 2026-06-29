using Asp.Versioning;
using CompX.Api.Common;
using CompX.Api.Extensions;
using CompX.Api.Security;
using CompX.Application.DependencyInjection;
using CompX.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Syncfusion.Licensing;

var builder = WebApplication.CreateBuilder(args);

var syncfusionLicenseKey = builder.Configuration["Syncfusion:LicenseKey"]?.Trim();
if (!string.IsNullOrWhiteSpace(syncfusionLicenseKey))
{
    SyncfusionLicenseProvider.RegisterLicense(syncfusionLicenseKey);
}

builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build());

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<RbacAuthorizationFilter>();

builder.Services.AddControllers(options =>
{
    options.Filters.AddService<RbacAuthorizationFilter>();
})
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    // ✅ FIX: prevents DTO name collisions like NurseDto vs NurseDto
    options.CustomSchemaIds(type => type.FullName);

    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CompX API",
        Version = "v1"
    });
});

var corsPolicyName = builder.Configuration["Origin"]?.Trim() ?? "compx-api";

var allowedOrigins = builder.Configuration["AllowedOrigins"]
    ?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
        else
        {
            policy.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin();
        }
    });
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

var frontendRoot = ResolveFrontendRootPath(
    app.Environment.ContentRootPath,
    builder.Configuration["Frontend:BuildOutputPath"]);

var localRouteRedirects = builder.Configuration
    .GetSection("LocalRouteRedirects")
    .GetChildren()
    .Select(section => new
    {
        Source = NormalizeConfiguredPath(section["Source"]),
        Destination = NormalizeConfiguredPath(section["Destination"])
    })
    .Where(redirect =>
        !string.IsNullOrWhiteSpace(redirect.Source) &&
        !string.IsNullOrWhiteSpace(redirect.Destination))
    .ToArray();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(corsPolicyName);

app.MapAngularApp("/app", frontendRoot);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

foreach (var redirect in localRouteRedirects)
{
    app.MapGet(redirect.Source!, () => Results.Redirect(redirect.Destination!))
        .AllowAnonymous()
        .WithOrder(-100);
}

app.MapReverseProxy()
    .AllowAnonymous();

app.Run();

static string ResolveFrontendRootPath(string contentRootPath, string? configuredPath)
{
    if (!string.IsNullOrWhiteSpace(configuredPath))
    {
        return Path.GetFullPath(
            Path.IsPathRooted(configuredPath)
                ? configuredPath
                : Path.Combine(contentRootPath, configuredPath));
    }

    return Path.GetFullPath(Path.Combine(
        contentRootPath,
        "..",
        "..",
        "..",
        "frontend",
        "dist",
        "comp-x",
        "browser"));
}

static string NormalizeConfiguredPath(string? path)
{
    if (string.IsNullOrWhiteSpace(path))
    {
        return string.Empty;
    }

    var trimmed = path.Trim();

    return trimmed.StartsWith('/')
        ? trimmed
        : $"/{trimmed}";
}
