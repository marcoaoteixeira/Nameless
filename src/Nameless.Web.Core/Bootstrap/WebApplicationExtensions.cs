using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Bootstrap;

namespace Nameless.Web.Bootstrap;

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
        /// <param name="timeout">
        ///     The maximum time to wait for the bootstrap process to complete,
        ///     in milliseconds.
        /// </param>
        public WebApplication UseBootstrap(int timeout = -1) {
            // The application needs to start before we can run the
            // bootstrap process, so we register a callback to be executed
            // once the application has started.
            self.Lifetime.ApplicationStarted.Register(
                callback: (state, _) => {
                    var executor = self.Services.GetService<IBootstrapExecutor>()
                                   ?? throw new BootstrappingException("Bootstrap services were not registered.");

                    using var cts = new CancellationTokenSource(
                        millisecondsDelay: Convert.ToInt32(state)
                    );

                    // Although this is an async method, we need to run it on
                    // a blocking manner here since the callback does not
                    // support async execution.
                    executor.ExecuteAsync(cts.Token)
                            .GetAwaiter()
                            .GetResult();
                },
                state: timeout
            );

            return self;
        }
    }
}