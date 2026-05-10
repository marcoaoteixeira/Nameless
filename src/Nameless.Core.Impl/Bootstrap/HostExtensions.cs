using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;

namespace Nameless.Bootstrap;

/// <summary>
///     Bootstrap <see cref="IHost"/> extension methods
/// </summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static class HostExtensions {
    extension<THost>(THost self) where THost : IHost {
        /// <summary>
        ///     Synchronously executes the <see cref="IBootstrapper"/> warmup.
        /// </summary>
        /// <param name="configure">
        ///     Optional delegate to configure <see cref="BootstrapWarmupOptions"/>.
        /// </param>
        public void Warmup(Action<BootstrapWarmupOptions>? configure) {
            self.WarmupAsync(configure).GetAwaiter().GetResult();
        }

        /// <summary>
        ///     Asynchronously executes the <see cref="IBootstrapper"/> warmup.
        /// </summary>
        /// <param name="configure">
        ///     Optional delegate to configure <see cref="BootstrapWarmupOptions"/>.
        /// </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task WarmupAsync(Action<BootstrapWarmupOptions>? configure) {
            var opts = ActionHelper.FromDelegate(configure);
            var bootstrapper = self.Services.GetRequiredService<IBootstrapper>();

            using var cts = new CancellationTokenSource(
                millisecondsDelay: opts.Timeout
            );

            return bootstrapper.ExecuteAsync(opts.Context, cts.Token);
        }

        /// <summary>
        ///     Asynchronously executes the <see cref="IBootstrapper"/> warmup.
        /// </summary>
        /// <param name="context">
        ///     The Bootstrap flow context.
        /// </param>
        /// <param name="cancellationToken">
        ///     The cancellation token.
        /// </param>
        /// <returns>
        ///     A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        public Task WarmupAsync(FlowContext? context = null, CancellationToken cancellationToken = default) {
            return self.Services
                       .GetRequiredService<IBootstrapper>()
                       .ExecuteAsync(context ?? [], cancellationToken);
        }
    }
}
