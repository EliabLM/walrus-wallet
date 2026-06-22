using Microsoft.Extensions.DependencyInjection;
using WalrusWallet.Domain.Common.Interfaces;
using WalrusWallet.Infrastructure.Services;

namespace WalrusWallet.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IAppInfoService, AppInfoService>();
        services.AddHealthChecks();
        //* Aquí se agregan los checks reales, ej:
        // .AddNpgSql(connectionSTring)
        // .AddRedis(redisConnection)

        //* Cuando se tenga PostgreSQL, Redis, etc., se instalan los paquetes correspondientes (AspNetCore.HealthChecks.NpgSql, AspNetCore.HealthChecks.Redis)

        return services;
    }
}

