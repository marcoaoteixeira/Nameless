namespace Nameless.WinApp.Infrastructure;

/// <summary>
///     Test seam for the process-level facts the self-update feature needs
///     (plan §3.4). Kept separate from <see cref="Application.IApplicationContext"/>
///     because these are process facts, not application configuration.
/// </summary>
public interface ISystemUpdateEnvironment {
    /// <summary>
    ///     Gets the install directory path (<see cref="AppContext.BaseDirectory"/>,
    ///     normalized, no trailing separator).
    /// </summary>
    string InstallDirectoryPath { get; }

    /// <summary>
    ///     Gets the path of the currently running executable
    ///     (<see cref="Environment.ProcessPath"/>).
    /// </summary>
    string ExecutablePath { get; }

    /// <summary>
    ///     Gets whether this build was published with self-update enabled
    ///     (release contract §3). A development build never carries the
    ///     marker and must never attempt to update itself.
    /// </summary>
    bool IsSelfUpdateEnabled { get; }
}
