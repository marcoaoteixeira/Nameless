using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Nameless.Caching;
using Nameless.Serialization;
using StackExchange.Redis;

namespace Nameless.Cache.Redis {

    public class RedisCache : ICache {
        #region Private Read-Only Fields

        private readonly IDatabaseAsync _database;
        private readonly ISerializer _serializer;

        #endregion

        #region Public Constructors

        public RedisCache (IConnectionMultiplexer connection, ISerializer serializer) {
            Prevent.ParameterNull (connection, nameof (connection));
            Prevent.ParameterNull (serializer, nameof (serializer));

            _database = connection.GetDatabase ();
            _serializer = serializer;
        }

        #endregion

        #region Private Methods

        private void SetExpirationDate (string key, CacheEntryOptions options) {
            if (options == null) { return; }
            
            // TODO: Set expiration keys absolute and sliding.
        }

        #endregion

        #region ICache Members

        public Task SetAsync (string key, object value, CacheEntryOptions options = null, CancellationToken token = default) {
            RedisValue redisValue;
            using (var memoryStream = new MemoryStream ()) {
                _serializer.Serialize (memoryStream, value);
                redisValue = RedisValue.CreateFrom (memoryStream);
            }

            return _database.StringSetAsync (key, redisValue);
        }

        public Task<T> GetAsync<T> (string key, CancellationToken token = default) {
            return _database
                .StringGetAsync (key)
                .ContinueWith (continuation => {
                    T result = default;
                    if (continuation.CanContinue ()) {
                        if (continuation.Result.HasValue) {
                            result = _serializer.Deserialize<T> (continuation.Result.ToString ().ToStream ());
                        }
                    }
                    return result;
                });
        }

        public Task RemoveAsync (string key, CancellationToken token = default) {
            return _database.KeyDeleteAsync (key);
        }

        #endregion
    }
}