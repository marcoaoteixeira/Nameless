using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Nameless.Caching.InMemory;

/// <summary>
/// Implementation of <see cref="ICache"/> for <see cref="MemoryCache"/>.
/// </summary>
public sealed class InMemoryCache : ICache, IDisposable {
    private MemoryCache? _memoryCache;
    private bool _disposed;

    ~InMemoryCache() {
        Dispose(disposing: false);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="key"/> or
    /// <paramref name="value"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// if <paramref name="key"/> is empty or white spaces.
    /// </exception>
    public Task<bool> SetAsync(string key, object value, CacheEntryOptions? opts = null, CancellationToken cancellationToken = default) {
        BlockAccessAfterDispose();

        Prevent.Argument.NullOrWhiteSpace(key, nameof(key));
        Prevent.Argument.Null(value, nameof(value));

        cancellationToken.ThrowIfCancellationRequested();

        var memoryCacheEntryOptions = CreateMemoryCacheEntryOptions(opts ?? CacheEntryOptions.Default);
        _ = GetMemoryCache().Set(key, value, memoryCacheEntryOptions);

        return Task.FromResult(true);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="key"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="key"/> is empty or white spaces.
    /// </exception>
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) {
        BlockAccessAfterDispose();

        Prevent.Argument.NullOrWhiteSpace(key, nameof(key));

        cancellationToken.ThrowIfCancellationRequested();

        var value = GetMemoryCache().Get(key);

        if (value is null) {
            return Task.FromResult<T?>(default);
        }

        if (value is not T result) {
            throw new InvalidOperationException($"Current cached value is not of type {typeof(T)}.");
        }

        return Task.FromResult<T?>(result);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="key"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// if <paramref name="key"/> is empty or white spaces.
    /// </exception>
    public Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default) {
        BlockAccessAfterDispose();

        Prevent.Argument.NullOrWhiteSpace(key, nameof(key));

        cancellationToken.ThrowIfCancellationRequested();

        GetMemoryCache().Remove(key);

        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void BlockAccessAfterDispose() {
        if (_disposed) {
            throw new ObjectDisposedException(nameof(InMemoryCache));
        }
    }

    private void Dispose(bool disposing) {
        if (_disposed) { return; }

        if (disposing) {
            _memoryCache?.Dispose();
        }

        _memoryCache = null;
        _disposed = true;
    }

    private IMemoryCache GetMemoryCache()
        => _memoryCache ??= new MemoryCache(new MemoryCacheOptions());

    private static void OnEviction(
        EvictionCallback evictionCallback,
        string key,
        object? value,
        string reason,
        CancellationTokenSource cts) {
        evictionCallback(key, value, reason);
        cts.Dispose();
    }

    private static MemoryCacheEntryOptions CreateMemoryCacheEntryOptions(CacheEntryOptions opts) {
        if (opts.ExpiresIn == default) {
            return new MemoryCacheEntryOptions();
        }

        var cts = new CancellationTokenSource(opts.ExpiresIn);
        var changeToken = new CancellationChangeToken(cts.Token);
        var result = new MemoryCacheEntryOptions();

        result.ExpirationTokens.Add(changeToken);

        if (opts.EvictionCallback is not null) {
            result.RegisterPostEvictionCallback(
                (key, value, reason, _)
                    => OnEviction(evictionCallback: opts.EvictionCallback,
                                  key: (string)key,
                                  value: value,
                                  reason: reason.ToString(),
                                  cts: cts));
        }

        return result;
    }
}