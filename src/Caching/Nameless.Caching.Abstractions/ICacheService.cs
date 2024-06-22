namespace Nameless.Caching {
    public interface ICacheService {
        #region Methods

        Task<bool> SetAsync(string key, object value, CacheEntryOptions opts, CancellationToken cancellationToken);

        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken);

        Task<bool> RemoveAsync(string key, CancellationToken cancellationToken);

        #endregion
    }
}