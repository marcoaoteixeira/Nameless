using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;

namespace Nameless.Bootstrap;

/// <summary>
///     Bootstrap <see cref="IHost"/> extension methods
/// </summary>
[ExcludeFromCodeCoverage]
public static class HostExtensions {
    extension<THost>(THost self) where THost : IHost {
        /// <summary>
        ///     Synchronously executes the <see cref="IBootstrapper"/> warmup.
        /// </summary>
        /// <param name="configure">
        ///     Optional delegate to configure <see cref="BootstrapOptions"/>.
        /// </param>
        public void Warmup(Action<BootstrapOptions>? configure) {
            self.WarmupAsync(configure).GetAwaiter().GetResult();
        }

        /// <summary>
        ///     Asynchronously executes the <see cref="IBootstrapper"/> warmup.
        /// </summary>
        /// <param name="configure">
        ///     Optional delegate to configure <see cref="BootstrapOptions"/>.
        /// </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task WarmupAsync(Action<BootstrapOptions>? configure) {
            var opts = ActionHelper.FromDelegate(configure);
            var bootstrapper = self.Services.GetRequiredService<IBootstrapper>();

            using var cts = new CancellationTokenSource(
                millisecondsDelay: opts.WarmupTimeout
            );

            return bootstrapper.RunAsync(cts.Token);
        }

        /// <summary>
        ///     Asynchronously executes the <see cref="IBootstrapper"/> warmup.
        /// </summary>
        /// <param name="cancellationToken">
        ///     The cancellation token.
        /// </param>
        /// <returns>
        ///     A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        public Task WarmupAsync(CancellationToken cancellationToken = default) {
            return self.Services
                       .GetRequiredService<IBootstrapper>()
                       .RunAsync(cancellationToken);
        }
    }
}
