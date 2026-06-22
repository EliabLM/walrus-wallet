using System.Reflection;
using WalrusWallet.Domain.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace WalrusWallet.Infrastructure.Services;

public sealed class AppInfoService : IAppInfoService
{
    private readonly IConfiguration _configuration;

    public AppInfoService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string Version =>
        Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion ?? "0.0.0";

    public string Environment =>
        _configuration["ASPNETCORE_ENVIRONMENT"] ?? "Unknown";

    public DateTime BuildDate =>
        System.IO.File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location);

}