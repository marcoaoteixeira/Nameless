using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.Autofac.Internals;

namespace Nameless.Autofac;

/// <summary>
///     <see cref="IServiceProvider" /> extensions methods.
/// </summary>
public static class ServiceProviderExtensions {
    /// <param name="self">
    ///     The current <see cref="IServiceProvider" /> instance.
    /// </param>
    extension(IServiceProvider self) {
        /// <summary>
        ///     Adds Autofac dispose action to the application lifecycle stop event.
        ///     Tear down the composition root and free all resources
        ///     when the application stops.
        /// </summary>
        public void UseAutofacDisposeHandler() {
            var lifetime = self.GetService<IHostApplicationLifetime>();

            if (lifetime is null) {
                self.GetLogger(typeof(ServiceProviderExtensions))
                    .HostApplicationLifetimeUnavailable(nameof(IHostApplicationLifetime));

                return;
            }

            var container = self.GetAutofacRoot();
            lifetime.ApplicationStopped.Register(container.Dispose);
        }
    }
}