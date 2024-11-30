using System.Text.Json;
using Microsoft.Extensions.Logging;
using Nameless.Caching.Redis.Internals;
using StackExchange.Redis;

namespace Nameless.Caching.Redis;

/// <summary>
/// Implementation of <see cref="ICache"/> for Redis.
/// </summary>
public sealed class RedisCache : ICache {
    private readonly IConnectionMultiplexerManager _connectionMultiplexerManager;
    private readonly ILogger<RedisCache> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="RedisCache"/>.
    /// </summary>
    /// <param name="connectionMultiplexerManager">The connection multiplexer manager.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="connectionMultiplexerManager"/> or
    /// <paramref name="logger"/> is <c>null</c>.
    /// </exception>
    public RedisCache(IConnectionMultiplexerManager connectionMultiplexerManager, ILogger<RedisCache> logger) {
        _connectionMultiplexerManager = Prevent.Argument.Null(connectionMultiplexerManager);
        _logger = Prevent.Argument.Null(logger);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="key"/> or
    /// <paramref name="value"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///if <paramref name="key"/> is empty or white spaces.
    /// </exception>
    public async Task<bool> SetAsync(string key, object value, CacheEntryOptions opts, CancellationToken cancellationToken) {
        Prevent.Argument.NullOrWhiteSpace(key);
        Prevent.Argument.Null(value);

        var json = JsonSerializer.Serialize(value);
        var expiry = opts.ExpiresIn != TimeSpan.Zero
            ? (TimeSpan?)opts.ExpiresIn
            : null;

        _logger.OnCondition(opts.EvictionCallback is not null)
               .CantUseEvictionCallback();

        cancellationToken.ThrowIfCancellationRequested();

        var database = await GetDatabaseAsync()
            .ConfigureAwait(continueOnCapturedContext: false);

        return await database.StringSetAsync(key: key,
                                             value: json,
                                             expiry: expiry,
                                             keepTtl: false,
                                             when: When.Always,
                                             flags: CommandFlags.None)
                             .ConfigureAwait(continueOnCapturedContext: false);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="key"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///if <paramref name="key"/> is empty or white spaces.
    /// </exception>
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken) {
        Prevent.Argument.NullOrWhiteSpace(key);

        cancellationToken.ThrowIfCancellationRequested();

        var database = await GetDatabaseAsync()
            .ConfigureAwait(continueOnCapturedContext: false);

        var value = await database.StringGetAsync(key: key, flags: CommandFlags.None)
                                  .ConfigureAwait(continueOnCapturedContext: false);

        return value.HasValue
            ? JsonSerializer.Deserialize<T>(value.ToString())
            : default;
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="key"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///if <paramref name="key"/> is empty or white spaces.
    /// </exception>
    public async Task<bool> RemoveAsync(string key, CancellationToken cancellationToken) {
        Prevent.Argument.NullOrWhiteSpace(key);

        cancellationToken.ThrowIfCancellationRequested();

        var database = await GetDatabaseAsync()
            .ConfigureAwait(continueOnCapturedContext: false);

        var result = await database.StringGetDeleteAsync(key: key, flags: CommandFlags.None)
                                   .ConfigureAwait(continueOnCapturedContext: false);

        return !result.IsNull;
    }

    private async Task<IDatabase> GetDatabaseAsync() {
        var multiplexer = await _connectionMultiplexerManager
                                .GetConnectionMultiplexerAsync()
                                .ConfigureAwait(continueOnCapturedContext: false);

        return multiplexer.GetDatabase();
    }
}