using System.Collections.Concurrent;
using Microsoft.Extensions.Options;

namespace Nameless.Reporting;

/// <summary>
///     Current implementation of <see cref="IStatusReportingHub"/>.
/// </summary>
public class StatusReportingHub : IStatusReportingHub {
    private readonly ConcurrentDictionary<CacheKey, StatusReporting> _channels = new();
    private readonly TimeProvider _timeProvider;
    private readonly IOptions<StatusReportingOptions> _options;

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="StatusReportingHub"/> class.
    /// </summary>
    /// <param name="timeProvider">
    ///     The time provider.
    /// </param>
    /// <param name="options">
    ///     The options.
    /// </param>
    public StatusReportingHub(TimeProvider timeProvider, IOptions<StatusReportingOptions> options) {
        _timeProvider = timeProvider;
        _options = options.Validate();
    }

    /// <inheritdoc />
    public IStatusReporter GetReporter(Type service, string? channelKey = null) {
        Throws.When.Null(service);

        return GetOrCreate(CacheKey.Create(service, channelKey));
    }

    /// <inheritdoc />
    public IStatusMonitor GetMonitor(Type service, string? channelKey = null) {
        Throws.When.Null(service);

        return GetOrCreate(CacheKey.Create(service, channelKey));
    }

    private StatusReporting GetOrCreate(CacheKey cacheKey) {
        return _channels.GetOrAdd(cacheKey, CreateStatusReporting);
    }

    private StatusReporting CreateStatusReporting(CacheKey cacheKey) {
        var reporting = new StatusReporting(
            cacheKey.ServiceName,
            cacheKey.Channel,
            _timeProvider,
            _options
        );

        reporting.Idle += () => EvictStatusReporting(cacheKey, reporting);

        return reporting;
    }

    private void EvictStatusReporting(CacheKey cacheKey, StatusReporting reporting) {
        // Compare-remove: only removes if the dictionary still holds THIS
        // instance for the key - protects against a race where the channel
        // was already evicted and recreated by a concurrent GetOrCreate.

        _channels.TryRemove(
            new KeyValuePair<CacheKey, StatusReporting>(cacheKey, reporting)
        );
    }

    internal readonly record struct CacheKey(string ServiceName, string? Channel) {
        internal static CacheKey Create(Type service, string? channel) {
            var serviceFullName = !string.IsNullOrWhiteSpace(service.Namespace)
                ? $"{service.Namespace}.{service.Name}"
                : service.Name;

            return new CacheKey(serviceFullName, channel);
        }
    }
}
