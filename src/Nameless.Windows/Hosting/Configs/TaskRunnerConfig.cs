using Microsoft.Extensions.Hosting;
using Nameless.Windows.Hosting.Wrappers;
using Nameless.Windows.TaskRunner;

namespace Nameless.Windows.Hosting.Configs;

/// <summary>
///     Task Runner Configuration
/// </summary>
public static class TaskRunnerConfig {
    /// <param name="self">
    ///     The current <see cref="WinHostFactory"/> instance.
    /// </param>
    extension(WinHostBuilder self) {
        /// <summary>
        ///     Configures the Task Runner services.
        /// </summary>
        /// <param name="settings">
        ///     The settings.
        /// </param>
        /// <returns>
        ///     The current <see cref="WinHostFactory"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public WinHostBuilder ConfigureTaskRunner(WinHostSettings settings) {
            if (settings.DisableTaskRunner) { return self; }

            self.ConfigureServices(
                services => services.RegisterTaskRunner()
            );

            return self;
        }
    }
}
