namespace Nameless.IO.Monitoring;

/// <summary>
///     Arguments for file watcher error events.
/// </summary>
/// <param name="Error">
///     The exception that caused the error.
/// </param>
public record FileMonitorErrorEventArgs(Exception Error);
