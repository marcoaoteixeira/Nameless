namespace Nameless.Caching {

    public interface ICache {

        #region Methods

        Task<object> SetAsync(string key, object value, CacheEntryOptions? opts = default, CancellationToken cancellationToken = default);
        Task<object?> GetAsync(string key, CancellationToken cancellationToken = default);
        Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default);
        Task<bool> RefreshAsync(string key, CancellationToken cancellationToken = default);

        #endregion
    }
}