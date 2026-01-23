using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Bootstrap;

namespace Nameless.Web;

/// <summary>
///     <see cref="WebApplication"/> extension methods.
/// </summary>
public static class WebApplicationExtensions {
    /// <param name="self">
    ///     The current <see cref="WebApplication"/> instance.
    /// </param>
    extension(WebApplication self) {
        /// <summary>
        ///     Runs the bootstrap process before initializing the application.
        /// </summary>
        /// <param name="url">
        ///     The URL to listen to if the server hasn't been configured
        ///     directly.
        /// </param>
        /// <param name="executeBootstrap">
        ///     Whether to execute the bootstrap process before running the
        ///     application.
        /// </param>
        /// <param name="timeout">
        ///     The maximum time to wait for the bootstrap process to complete,
        ///     in milliseconds.
        /// </param>
        public void Run(string? url = null, bool executeBootstrap = false, int timeout = -1) {
            if (executeBootstrap) {
                // The application needs to start before we can run the
                // bootstrap process, so we register a callback to be executed
                // once the application has started.
                self.Lifetime.ApplicationStarted.Register(
                    callback: (state, token) => self.ExecuteBootstrapAsync(state, token),
                    state: timeout
                );
            }

            self.Run(url);
        }
        
        private Task ExecuteBootstrapAsync(object? state, CancellationToken _) {
            var timeout = Convert.ToInt32(state);

            var executor = self.Services.GetService<IBootstrapExecutor>()
                           ?? throw new BootstrappingException("Bootstrap services were not registered.");

            using var cts = new CancellationTokenSource(timeout);

            return executor.ExecuteAsync(cts.Token);
        }
    }
}