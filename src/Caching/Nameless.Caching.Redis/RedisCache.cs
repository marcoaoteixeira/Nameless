using System.Text.Json;
using Microsoft.Extensions.Logging;
using Nameless.Caching.Redis.Internals;
using StackExchange.Redis;

namespace Nameless.Caching.Redis;

/// <summary>
/// Implementation of <see cref="ICache"/> for Redis.
/// </summary>
public sealed class RedisCache : ICache, IDisposable {
    private readonly ConfigurationOptions _configurationOptions;
    private readonly ILogger<RedisCache> _logger;

    private ConnectionMultiplexer? _multiplexer;
    private IDatabase? _database;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of <see cref="RedisCache"/>.
    /// </summary>
    /// <param name="configurationOptionsFactory">Redis configuration options.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="configurationOptionsFactory"/> or
    /// <paramref name="logger"/> is <c>null</c>.
    /// </exception>
    public RedisCache(IConfigurationOptionsFactory configurationOptionsFactory, ILogger<RedisCache> logger) {
        _configurationOptions = Prevent.Argument
                                       .Null(configurationOptionsFactory)
                                       .CreateConfigurationOptions();
        _logger = Prevent.Argument.Null(logger);
    }

    ~RedisCache() {
        Dispose(disposing: false);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="key"/> or
    /// <paramref name="value"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///if <paramref name="key"/> is empty or white spaces.
    /// </exception>
    public Task<bool> SetAsync(string key, object value, CacheEntryOptions opts, CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        Prevent.Argument.NullOrWhiteSpace(key);
        Prevent.Argument.Null(value);

        var json = JsonSerializer.Serialize(value);
        var expiry = opts.ExpiresIn != TimeSpan.Zero
            ? (TimeSpan?)opts.ExpiresIn
            : null;

        if (opts.EvictionCallback is not null) {
            _logger.CantUseEvictionCallback();
        }

        cancellationToken.ThrowIfCancellationRequested();

        return GetDatabase().StringSetAsync(key: key,
                                            value: json,
                                            expiry: expiry,
                                            keepTtl: false,
                                            when: When.Always,
                                            flags: CommandFlags.None);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="key"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///if <paramref name="key"/> is empty or white spaces.
    /// </exception>
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        Prevent.Argument.NullOrWhiteSpace(key);

        cancellationToken.ThrowIfCancellationRequested();

        var value = await GetDatabase()
                          .StringGetAsync(key: key, flags: CommandFlags.None)
                          .ConfigureAwait(false);

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
        BlockAccessAfterDispose();

        Prevent.Argument.NullOrWhiteSpace(key);

        cancellationToken.ThrowIfCancellationRequested();

        var result = await GetDatabase()
                           .StringGetDeleteAsync(key: key, flags: CommandFlags.None)
                           .ConfigureAwait(false);

        return !result.IsNull;
    }

    /// <inheritdoc />
    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private IConnectionMultiplexer GetMultiplexer()
        => _multiplexer ??= ConnectionMultiplexer.Connect(
            configuration: _configurationOptions
        );

    private IDatabase GetDatabase()
        => _database ??= GetMultiplexer().GetDatabase();

    private void BlockAccessAfterDispose() {
        if (_disposed) {
            throw new ObjectDisposedException(nameof(RedisCache));
        }
    }

    private void Dispose(bool disposing) {
        if (_disposed) { return; }

        if (disposing) {
            _multiplexer?.Dispose();
        }

        _database = null;
        _multiplexer = null;
        _disposed = true;
    }
}