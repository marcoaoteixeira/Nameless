using Nameless.WPF.GitHub;

namespace Nameless.WPF.Hosting.Configs;

/// <summary>
///     GitHub HTTP Client Configuration
/// </summary>
public static class GitHubHttpClientConfig {
    /// <param name="self">
    ///     The current <see cref="WinHostFactory"/> instance.
    /// </param>
    extension(WinHostBuilder self) {
        /// <summary>
        ///     Configures the GitHub HTTP Client services.
        /// </summary>
        /// <param name="settings">
        ///     The settings.
        /// </param>
        /// <returns>
        ///     The current <see cref="WinHostFactory"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public WinHostBuilder RegisterGitHubHttpClient(WinHostSettings settings) {
            if (settings.DisableGitHubHttpClient) { return self; }

            self.ConfigureServices(
                (ctx, services) => services.RegisterGitHubHttpClient(ctx.Configuration)
            );

            return self;
        }
    }
}
