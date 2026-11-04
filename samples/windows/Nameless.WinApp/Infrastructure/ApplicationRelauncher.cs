using System.Diagnostics;

namespace Nameless.WinApp.Infrastructure;

/// <summary>
///     Default <see cref="IApplicationRelauncher"/> implementation.
/// </summary>
public sealed class ApplicationRelauncher : IApplicationRelauncher {
    /// <inheritdoc />
    public void RelaunchAndShutdown(string executablePath, string workingDirectoryPath) {
        try {
            using (Process.Start(new ProcessStartInfo(executablePath) {
                UseShellExecute = false,
                WorkingDirectory = workingDirectoryPath
            })) { }
        }
        catch (Exception ex) when (ex is InvalidOperationException or System.ComponentModel.Win32Exception) {
            throw new InvalidOperationException($"Could not start '{executablePath}'.", ex);
        }

        System.Windows.Application.Current.Shutdown();
    }
}
