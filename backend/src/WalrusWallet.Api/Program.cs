using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using WalrusWallet.Api.Middleware;
using WalrusWallet.Api.Options;
using WalrusWallet.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddInfrastructure();
builder.Services.Configure<AppInfoOptions>(builder.Configuration.GetSection("AppInfo"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapGet("/version", (IOptions<AppInfoOptions> appInfo, IHostEnvironment env) => Results.Ok(new
{
    version = appInfo.Value.Version,
    commitHash = appInfo.Value.CommitHash,
    buildDate = appInfo.Value.BuildDate,
    environment = env.EnvironmentName
}));

app.Run();
