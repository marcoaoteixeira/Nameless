using System.Threading;
using System.Threading.Tasks;

namespace Nameless.Caching {
    public interface ICache {
        #region Methods

        Task SetAsync (string key, object value, CacheEntryOptions options = null, CancellationToken token = default);

        Task<T> GetAsync<T> (string key, CancellationToken token = default);

        Task RemoveAsync (string key, CancellationToken token = default);

        #endregion
    }
}