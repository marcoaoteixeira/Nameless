namespace Nameless.IO.Monitoring;

/// <summary>
///     Represents an asynchronous handler for a file watcher event.
/// </summary>
/// <typeparam name="TArgs">
///     The event arguments type.
/// </typeparam>
/// <param name="args">
///     The event arguments.
/// </param>
/// <param name="cancellationToken">
///     The cancellation token.
/// </param>
public delegate ValueTask FileMonitorDelegate<in TArgs>(TArgs args, CancellationToken cancellationToken);
