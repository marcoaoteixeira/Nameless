using Microsoft.Extensions.Logging;

namespace Nameless.Testing.Tools.Logging;

/// <summary>
///     Represents a single log call captured by <see cref="InMemoryLogger"/>.
/// </summary>
/// <param name="Level">
///     The log level.
/// </param>
/// <param name="EventId">
///     The event identifier.
/// </param>
/// <param name="Message">
///     The formatted log message.
/// </param>
/// <param name="Exception">
///     The exception associated with the log call, if any.
/// </param>
/// <param name="State">
///     The original state object passed to the log call.
/// </param>
/// <param name="Scopes">
///     Snapshot of the active scope states (outer to inner) at the
///     moment this entry was logged.
/// </param>
public sealed record LogEntry(
    LogLevel Level,
    EventId EventId,
    string Message,
    Exception? Exception,
    object? State,
    IReadOnlyList<object?> Scopes
);
