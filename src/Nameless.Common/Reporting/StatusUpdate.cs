namespace Nameless.Reporting;

/// <summary>
///     A single, point-in-time status snapshot from a reporting service.
///     Intended to be shown live (e.g. in a UI or pushed over SignalR)
///     and then discarded - this is not a log entry and is not meant
///     to be persisted.
/// </summary>
public sealed record StatusUpdate(
    string ServiceName,
    string Message,
    StatusLevel Level,
    DateTimeOffset Timestamp
);