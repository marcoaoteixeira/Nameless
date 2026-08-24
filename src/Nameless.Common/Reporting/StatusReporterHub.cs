using System.Collections.Concurrent;

namespace Nameless.Reporting;

/// <summary>
///     Default implementation of <see cref="IStatusReporterHub"/> /
///     <see cref="IStatusMonitorHub"/>, keeping one
///     <see cref="StatusReporter{TService}"/> channel per key alive for as
///     long as it is either active or still has consumers watching it after
///     completing. A finished channel is evicted from the registry only once
///     it has both reached a terminal state and lost its last subscriber -
///     a subsequent <see cref="GetOrCreate"/> for the same key then produces
///     a brand-new, empty channel.
/// </summary>
/// <remarks>
///     This type is public (rather than internal) so it can be unit-tested
///     and constructed directly where useful, but it is intentionally NOT
///     the type application code should depend on. Use
///     <see cref="ServiceCollectionExtensions.RegisterStatusReporting{TService}"/>
///     to register it, which seals off unkeyed resolution of this concrete
///     type and only exposes it through
///     <see cref="IStatusReporterHub"/> / <see cref="IStatusMonitorHub"/>.
/// </remarks>
public sealed class StatusReporterHub : IStatusReporterHub, IStatusMonitorHub, IDisposable {
    private readonly int _bufferSize;
    private readonly ConcurrentDictionary<CacheKey, IDisposable> _channels = new();

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="StatusReporterHub"/> class.
    /// </summary>
    /// <param name="bufferSize">
    ///     The number of most-recent status updates replayed to new
    ///     subscribers of each channel created by this hub.
    /// </param>
    public StatusReporterHub(int bufferSize = 10) {
        _bufferSize = Throws.When.LowerThan(bufferSize, compare: 0);
    }

    /// <inheritdoc />
    public IStatusReporter<TService> GetOrCreate<TService>(string channelKey) {
        Throws.When.Null(channelKey);

        var cacheKey = CacheKey.Create<TService>(channelKey);
        var item = _channels.GetOrAdd(cacheKey, CreateChannel<TService>);

        return (IStatusReporter<TService>)item;
    }

    /// <inheritdoc />
    public bool TryGet<TService>(string channelKey, out IStatusMonitor<TService>? output) {
        output = null;

        var cacheKey = CacheKey.Create<TService>(channelKey);

        if (_channels.TryGetValue(cacheKey, out var item) && item is IStatusMonitor<TService> monitor) {
            output = monitor;
        }

        return output is not null;
    }

    private StatusReporter<TService> CreateChannel<TService>(CacheKey cacheKey) {
        var reporter = new StatusReporter<TService>(cacheKey.Channel, _bufferSize);

        reporter.Idle += () => Evict(cacheKey, reporter);

        return reporter;
    }

    private void Evict<TService>(CacheKey cacheKey, StatusReporter<TService> reporter) {
        // Compare-remove: only removes if the dictionary still holds THIS
        // instance for the key - protects against a race where the channel
        // was already evicted and recreated by a concurrent GetOrCreate.

        _channels.TryRemove(
            new KeyValuePair<CacheKey, IDisposable>(cacheKey, reporter)
        );
    }

    /// <summary>
    ///     Disposes every channel currently registered with this hub. Safe
    ///     to call on host shutdown; not required for correctness of
    ///     <see cref="GetOrCreate"/>/<see cref="TryGet"/> while the process
    ///     runs.
    /// </summary>
    public void Dispose() {
        foreach (var reporter in _channels.Values) {
            reporter.Dispose();
        }
    }

    internal readonly record struct CacheKey(string Service, string Channel) {
        internal static CacheKey Create<TService>(string channel) {
            var serviceFullName = !string.IsNullOrWhiteSpace(typeof(TService).Namespace)
                ? $"{typeof(TService).Namespace}.{typeof(TService).Name}"
                : typeof(TService).Name;

            return new CacheKey(serviceFullName, channel);
        }
    }
}
