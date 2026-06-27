namespace Nameless.IO.Monitoring;

/// <summary>
///     Arguments for file watcher events (created, deleted, renamed, changed).
/// </summary>
/// <param name="CurrentFilePath">
///     The current (post-event) path of the file. For deleted events this is
///     the path of the file that was removed.
/// </param>
/// <param name="PreviousFilePath">
///     The previous path of the file. Only set for rename events;
///     <see langword="null"/> otherwise.
/// </param>
public record FileWatcherEventArgs(string CurrentFilePath, string? PreviousFilePath);
