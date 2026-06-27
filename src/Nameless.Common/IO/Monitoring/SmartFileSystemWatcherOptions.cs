using Nameless.Resilience;

namespace Nameless.IO.Monitoring;

/// <summary>
///     Configuration options for <see cref="SmartFileSystemWatcher"/>.
/// </summary>
public sealed class SmartFileSystemWatcherOptions {
    /// <summary>
    ///     Gets or sets the root directory path used when constructing a
    ///     <c>PhysicalFileProvider</c> via the DI registration helper.
    /// </summary>
    public string RootDirectory { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the sub-path within the provider to watch.
    ///     Defaults to <see cref="string.Empty"/> (provider root).
    /// </summary>
    public string SubPath { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the glob pattern used to filter watched files.
    ///     Defaults to <c>*.*</c>.
    /// </summary>
    public string Filter { get; set; } = "*.*";

    /// <summary>
    ///     Gets or sets the delay between lock probe attempts.
    ///     Defaults to 200 ms.
    /// </summary>
    public TimeSpan LockProbeDelay { get; set; } = TimeSpan.FromMilliseconds(200);

    /// <summary>
    ///     Gets or sets the maximum number of exclusive-lock probe attempts
    ///     before the watcher fires an error callback.
    ///     Defaults to 10.
    /// </summary>
    public int LockProbeMaxAttempts { get; set; } = 10;

    /// <summary>
    ///     Gets or sets whether to retry re-registering the change token on
    ///     failure.
    ///     Defaults to <see langword="false"/>.
    /// </summary>
    public bool EnableRetry { get; set; } = false;

    /// <summary>
    ///     Gets or sets the retry policy configuration used when
    ///     <see cref="EnableRetry"/> is <see langword="true"/>.
    ///     When <see langword="null"/>, a default configuration is used.
    /// </summary>
    public RetryPolicyConfiguration? RetryPolicy { get; set; }
}
