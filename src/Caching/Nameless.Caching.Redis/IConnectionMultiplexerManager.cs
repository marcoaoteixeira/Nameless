using StackExchange.Redis;

namespace Nameless.Caching.Redis;

public interface IConnectionMultiplexerManager {
    Task<IConnectionMultiplexer> GetConnectionMultiplexerAsync();
}