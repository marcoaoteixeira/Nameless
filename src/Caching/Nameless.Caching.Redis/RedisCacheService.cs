using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using StackExchange.Redis;

namespace Nameless.Caching.Redis {
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

        public RedisCacheService(ConfigurationOptions configurationOptions)
            : this(configurationOptions, NullLogger<RedisCacheService>.Instance) { }

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

        public Task<bool> SetAsync(string key, object value, CacheEntryOptions opts = default, CancellationToken cancellationToken = default) {
            BlockAccessAfterDispose();

            cancellationToken.ThrowIfCancellationRequested();

            var json = JsonSerializer.Serialize(value);
            var expiry = opts.ExpiresIn != default
                ? (TimeSpan?)opts.ExpiresIn
                : null;

            if (opts.EvictionCallback is not null) {
                _logger.LogInformation($"It's not possible to configure eviction callbacks for {nameof(RedisCacheService)}.");
            }

            return GetDatabase().StringSetAsync(key: key,
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
