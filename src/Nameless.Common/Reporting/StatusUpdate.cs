namespace Nameless.Reporting;

/// <summary>
///     A single, point-in-time status snapshot from a reporting service.
///     Intended to be shown live (e.g. in a UI or pushed over SignalR)
///     and then discarded - this is not a log entry and is not meant
///     to be persisted.
/// </summary>
public sealed record StatusUpdate {
    /// <summary>
    ///     Gets the service name.
    /// </summary>
    public string ServiceName { get; }

    /// <summary>
    ///     Gets the channel key.
    /// </summary>
    public string? ChannelKey { get; }

    /// <summary>
    ///     Gets the status message
    /// </summary>
    public string Message { get; }

    /// <summary>
    ///     Gets the status level.
    /// </summary>
    public StatusLevel Level { get; }
    
    /// <summary>
    ///     Gets the timestamp.
    /// </summary>
    public DateTimeOffset Timestamp { get; }
    
    /// <summary>
    ///     Gets the metadata.
    /// </summary>
    public IReadOnlyDictionary<string, string> Metadata { get; }

    /// <summary>
    ///     Initializes a new instance of <see cref="StatusUpdate"/> class.
    /// </summary>
    public StatusUpdate(string serviceName, string message, string? channelKey = null, StatusLevel level = StatusLevel.Info, DateTimeOffset timestamp = default, Dictionary<string, string>? metadata = null) {
        ServiceName = Throws.When.NullOrWhiteSpace(serviceName);
        ChannelKey = channelKey;
        Message = Throws.When.NullOrWhiteSpace(message);
        Level = level;
        Timestamp = timestamp;
        Metadata = metadata ?? [];
    }

    /// <summary>
    ///     Record destructor.
    /// </summary>
    public void Deconstruct(out string serviceName, out string? channelKey, out string message, out StatusLevel level, out DateTimeOffset timestamp, out IReadOnlyDictionary<string, string> metadata) {
        serviceName = ServiceName;
        channelKey = ChannelKey;
        message = Message;
        level = Level;
        timestamp = Timestamp;
        metadata = Metadata;
    }
}