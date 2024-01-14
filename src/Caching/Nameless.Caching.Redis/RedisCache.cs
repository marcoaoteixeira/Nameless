using System.Text.Json;
using StackExchange.Redis;

namespace Nameless.Caching.Redis {
    public sealed class RedisCache : ICache, IDisposable {
        #region Private Read-Only Fields

        private readonly IConfigurationFactory _configurationFactory;

        #endregion

        #region Private Fields

        private IConnectionMultiplexer? _multiplexer;
        private IDatabase? _database;
        private bool _disposed;

        #endregion

        #region Public Constructors

        public RedisCache(IConfigurationFactory configurationFactory) {
            _configurationFactory = Guard.Against.Null(configurationFactory, nameof(configurationFactory));
        }

        #endregion

        #region Private Methods

        private IConnectionMultiplexer GetMultiplexer()
            => _multiplexer ??= ConnectionMultiplexer.Connect(
                configuration: _configurationFactory.CreateConfigurationOptions()
            );

        private IDatabase GetDatabase()
            => _database ??= GetMultiplexer().GetDatabase();

        private void BlockAccessAfterDispose() {
            if (_disposed) {
                throw new ObjectDisposedException(typeof(RedisCache).Name);
            }
        }

        private void Dispose(bool disposing) {
            if (_disposed) {
                return;
            }

            if (disposing) {
                _multiplexer?.Dispose();
            }

            _database = null;
            _multiplexer = null;
            _disposed = true;
        }

        #endregion

        #region ICache Members

        public Task<bool> SetAsync(string key, object value, CacheEntryOptions? opts, CancellationToken cancellationToken = default) {
            BlockAccessAfterDispose();

            cancellationToken.ThrowIfCancellationRequested();

            var json = JsonSerializer.Serialize(value);
            var expiry = opts is not null && opts.ExpiresIn != default
                ? (TimeSpan?)opts.ExpiresIn
                : null;

            return GetDatabase().StringSetAsync(
                key: key,
                value: json,
                expiry: expiry,
                keepTtl: false,
                when: When.Always,
                flags: CommandFlags.None
            );
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
