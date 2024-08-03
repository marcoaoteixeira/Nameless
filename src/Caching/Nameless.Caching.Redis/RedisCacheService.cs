using System.Text.Json;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Nameless.Caching.Redis {
    /// <summary>
    /// Implementation of <see cref="ICacheService"/> for Redis.
    /// </summary>
    public sealed class RedisCacheService : ICacheService, IDisposable {
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

        /// <summary>
        /// Initializes a new instance of <see cref="RedisCacheService"/>.
        /// </summary>
        /// <param name="configurationOptions">Redis configuration options.</param>
        /// <param name="logger">The logger.</param>
        public RedisCacheService(ConfigurationOptions configurationOptions, ILogger<RedisCacheService> logger) {
            _configurationOptions = Guard.Against.Null(configurationOptions, nameof(configurationOptions));
            _logger = Guard.Against.Null(logger, nameof(logger));
        }

        #endregion

        #region Destructor

        ~RedisCacheService() {
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
                throw new ObjectDisposedException(nameof(RedisCacheService));
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

        /// <inheritdoc />
        public Task<bool> SetAsync(string key, object value, CacheEntryOptions opts = default, CancellationToken cancellationToken = default) {
            BlockAccessAfterDispose();

            var json = JsonSerializer.Serialize(value);
            var expiry = opts.ExpiresIn != default
                ? (TimeSpan?)opts.ExpiresIn
                : null;

            if (opts.EvictionCallback is not null) {
                _logger.LogInformation($"It's not possible to configure eviction callbacks for {nameof(RedisCacheService)}.");
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
