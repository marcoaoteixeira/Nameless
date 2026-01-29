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
        /// <param name="configure">
        ///     A configuration delegate to help configure the bootstrap
        ///     infrastructure.
        /// </param>
        public WebApplication UseBootstrap(Action<BootstrapExecutionOptions>? configure = null) {
            var innerConfigure = configure ?? (_ => { });
            var options = new BootstrapExecutionOptions();

            innerConfigure(options);

            // The application needs to start before we can run the
            // bootstrap process, so we register a callback to be executed
            // once the application has started.
            self.Lifetime.ApplicationStarted.Register(self.ExecuteBootstrap, options);

            return self;
        }

        private void ExecuteBootstrap(object? state, CancellationToken _) {
            var opts = state as BootstrapExecutionOptions ?? new BootstrapExecutionOptions();
            var executor = self.Services.GetService<IBootstrapper>()
                           ?? throw new BootstrapException("Bootstrap services were not registered.");

            using var cts = new CancellationTokenSource(
                millisecondsDelay: opts.Timeout
            );

            // Although this is an async method, we need to run it on
            // a blocking manner here since the callback does not
            // support async execution.
            executor.ExecuteAsync(opts.Context, cts.Token)
                    .GetAwaiter()
                    .GetResult();
        }
    }
}

public class BootstrapExecutionOptions {
    public FlowContext Context { get; set; } = [];
    public int Timeout { get; set; } = -1;
}