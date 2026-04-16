using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;

namespace Nameless.Bootstrap;

/// <summary>
///     Bootstrap <see cref="IHost"/> extension methods
/// </summary>
public static class HostExtensions {
    extension<THost>(THost self) where THost : IHost {
        public void Warmup(Action<WarmupOptions>? execution) {
            self.WarmupAsync(execution).GetAwaiter().GetResult();
        }

        public Task WarmupAsync(Action<WarmupOptions>? execution) {
            var opts = ActionHelper.FromDelegate(execution);
            var bootstrapper = self.Services.GetRequiredService<IBootstrapper>();

            using var cts = new CancellationTokenSource(
                millisecondsDelay: opts.Timeout
            );

            return bootstrapper.ExecuteAsync(opts.Context, cts.Token);
        }
    }
}
