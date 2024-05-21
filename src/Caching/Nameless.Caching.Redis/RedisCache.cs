using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using StackExchange.Redis;

namespace Nameless.Caching.Redis {
    public sealed class RedisCache : ICache, IDisposable {
        #region Private Read-Only Fields

        private readonly ConfigurationOptions _configurationOptions;
        private readonly ILogger _logger;

        #endregion

        #region Private Fields

        private ConnectionMultiplexer? _multiplexer;
        private IDatabase? _database;
        private bool _disposed;

        #endregion

        #region Public Constructors

        public RedisCache(ConfigurationOptions configurationOptions)
            : this(configurationOptions, NullLogger<RedisCache>.Instance) { }

        public RedisCache(ConfigurationOptions configurationOptions, ILogger<RedisCache> logger) {
            _configurationOptions = Guard.Against.Null(configurationOptions, nameof(configurationOptions));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        #endregion

        #region Destructor

        ~RedisCache() {
            Dispose(disposing: false);
        }

        #endregion

        #region Private Methods

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

        #endregion

        #region ICache Members

        public Task<bool> SetAsync(string key, object value, CacheEntryOptions? opts = null, CancellationToken cancellationToken = default) {
            BlockAccessAfterDispose();

            cancellationToken.ThrowIfCancellationRequested();

            var innerOpts = opts ?? CacheEntryOptions.Empty;
            var json = JsonSerializer.Serialize(value);
            var expiry = innerOpts.ExpiresIn != default
                ? (TimeSpan?)innerOpts.ExpiresIn
                : null;

            if (innerOpts.EvictionCallback is not null) {
                _logger.LogInformation($"It's not possible to configure eviction callbacks for {nameof(RedisCache)}.");
            }

            return GetDatabase()
                .StringSetAsync(key: key,
                                value: json,
                                expiry: expiry,
                                keepTtl: false,
                                when: When.Always,
                                flags: CommandFlags.None);
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) {
            BlockAccessAfterDispose();

            cancellationToken.ThrowIfCancellationRequested();

            var value = await GetDatabase()
                .StringGetAsync(key: key, flags: CommandFlags.None)
                .ConfigureAwait(false);

            return value.HasValue
                ? JsonSerializer.Deserialize<T>(value.ToString())
                : default;
        }

        public async Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default) {
            BlockAccessAfterDispose();

            cancellationToken.ThrowIfCancellationRequested();

            var result = await GetDatabase()
                .StringGetDeleteAsync(key: key, flags: CommandFlags.None)
                .ConfigureAwait(false);

            return !result.IsNull;
        }

        #endregion

        #region IDisposable Members

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
