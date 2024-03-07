using Microsoft.Extensions.DependencyInjection;
using MySalon.Domain;
using MySalon.Domain.Services;
using Trolley;
using Trolley.MySqlConnector;

namespace MySalon;

public static class Startup
{
    public static void AddDomainServices(this IServiceCollection services)
    {
        services.AddSingleton(f =>
        {
            var builder = new OrmDbFactoryBuilder()
            .Register<MySqlProvider>("salon", "Server=localhost;Database=salon;Uid=root;password=123456;charset=utf8mb4", true)
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
