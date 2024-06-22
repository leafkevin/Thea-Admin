using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheaAdmin.Domain;
using TheaAdmin.Domain.Services;
using Trolley;
using Trolley.MySqlConnector;

namespace TheaAdmin;

public static class Startup
{
    public static void AddDomainServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(f =>
        {
            var connectionString = configuration["ConnectionStrings:default"];
            var builder = new OrmDbFactoryBuilder()
            .Register<MySqlProvider>("default", connectionString, true)
            .Configure<MySqlProvider, ModelConfiguration>();
            return builder.Build();
        });
        services.AddSingleton<AccountService>();
        services.AddSingleton<ProfileService>();
        services.AddSingleton<TokenService>();
        services.AddSingleton<DepositService>();
        services.AddSingleton<OrderService>();
    }
}
