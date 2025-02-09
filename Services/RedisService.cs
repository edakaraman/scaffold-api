using StackExchange.Redis;
using System.Threading.Tasks;

public class RedisService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IDatabase _database;

    public RedisService(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _database = _connectionMultiplexer.GetDatabase();
    }

    public async Task SetStringAsync(string key, string value)
    {
        await _database.StringSetAsync(key, value);
    }

    public async Task<string?> GetStringAsync(string key)
    {
        RedisValue value = await _database.StringGetAsync(key);

        if (value.IsNull)
        {
            return null; 
        }

        return value;
    }
}
