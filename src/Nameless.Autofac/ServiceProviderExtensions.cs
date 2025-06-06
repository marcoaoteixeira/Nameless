using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Nameless.Autofac;

/// <summary>
///     <see cref="IServiceProvider" /> extensions methods.
/// </summary>
public static class ServiceProviderExtensions {
    /// <summary>
    ///     Adds Autofac dispose action to the application lifecycle stop event.
    ///     Tear down the composition root and free all resources
    ///     when the application stops.
    /// </summary>
    /// <param name="self">The current <see cref="IServiceProvider" /> instance.</param>
    public static void UseAutofacDisposeHandler(this IServiceProvider self) {
        var lifetime = self.GetService<IHostApplicationLifetime>();

        if (lifetime is null) {
            self.GetLogger(typeof(ServiceProviderExtensions))
                .HostApplicationLifetimeUnavailable(typeof(IHostApplicationLifetime));

            return;
        }

        var container = self.GetAutofacRoot();
        lifetime.ApplicationStopped.Register(container.Dispose);
    }
}