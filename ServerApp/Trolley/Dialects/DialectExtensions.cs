using Trolley.MySqlConnector;

namespace Trolley;

public static class DialectExtensions
{
    public static IMySqlRepository Create(this IOrmDbFactory dbFactory, string dbKey = null)
        => dbFactory.CreateRepository(dbKey) as IMySqlRepository;
}