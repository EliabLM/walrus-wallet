using Microsoft.Extensions.DependencyInjection;

namespace WalrusWallet.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddHealthChecks();
        //* Aquí se agregan los checks reales, ej:
        // .AddNpgSql(connectionSTring)
        // .AddRedis(redisConnection)

        //* Cuando se tenga PostgreSQL, Redis, etc., se instalan los paquetes correspondientes (AspNetCore.HealthChecks.NpgSql, AspNetCore.HealthChecks.Redis)

        return services;
    }
}

