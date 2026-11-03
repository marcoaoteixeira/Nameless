namespace Nameless.WinApp.Infrastructure;

/// <summary>
///     Test seam for the "start the new process, then shut this one down"
///     step the self-update view model runs after a completed update
///     (plan §3.5). Kept separate so the view model's relaunch path is
///     unit-testable without a real <see cref="System.Diagnostics.Process"/>
///     or <see cref="System.Windows.Application"/>.
/// </summary>
public interface IApplicationRelauncher {
    /// <summary>
    ///     Starts <paramref name="executablePath"/> as a new process and
    ///     shuts the current application down.
    /// </summary>
    /// <param name="executablePath">
    ///     The path of the executable to start.
    /// </param>
    /// <param name="workingDirectoryPath">
    ///     The working directory for the new process.
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///     When the new process could not be started. The caller must not
    ///     shut the current application down in that case.
    /// </exception>
    void RelaunchAndShutdown(string executablePath, string workingDirectoryPath);
}
