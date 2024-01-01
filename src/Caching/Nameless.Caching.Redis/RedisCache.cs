using System.Text.Json;
using StackExchange.Redis;

namespace Nameless.Caching.Redis {
    public sealed class RedisCache : ICache {
        #region Private Read-Only Fields

        private readonly IDatabase _database;

        #endregion

        #region Public Constructors

        public RedisCache(IDatabase database) {
            _database = Guard.Against.Null(database, nameof(database));
        }

        #endregion

        #region ICache Members

        public Task<bool> SetAsync(
            string key,
            object value,
            CacheEntryOptions? opts = null,
            CancellationToken cancellationToken = default
        ) {
            cancellationToken.ThrowIfCancellationRequested();

            var json = JsonSerializer.Serialize(value);
            var expiry = opts is not null && opts.ExpiresIn != default
                ? (TimeSpan?)opts.ExpiresIn
                : null;

            return _database.StringSetAsync(
                key: key,
                value: json,
                expiry: expiry,
                keepTtl: false,
                when: When.Always,
                flags: CommandFlags.None
            );
        }

        public async Task<T?> GetAsync<T>(
            string key,
            CancellationToken cancellationToken = default
        ) {
            cancellationToken.ThrowIfCancellationRequested();

            var value = await _database
                .StringGetAsync(key: key, flags: CommandFlags.None)
                .ConfigureAwait(false);

            return value.HasValue
                ? JsonSerializer.Deserialize<T>(value.ToString())
                : default;
        }

        public async Task<bool> RemoveAsync(
            string key,
            CancellationToken cancellationToken = default
        ) {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _database
                .StringGetDeleteAsync(key: key, flags: CommandFlags.None)
                .ConfigureAwait(false);

            return !result.IsNull;
        }

        #endregion
    }
}
