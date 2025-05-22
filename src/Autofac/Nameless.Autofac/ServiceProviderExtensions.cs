using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Nameless.Autofac;

/// <summary>
/// <see cref="IServiceProvider"/> extensions methods.
/// </summary>
public static class ServiceProviderExtensions {
    /// <summary>
    /// Adds Autofac dispose action to the application lifecycle stop event.
    /// Tear down the composition root and free all resources
    /// when the application stops.
    /// </summary>
    /// <param name="self">The current <see cref="IServiceProvider"/> instance.</param>
    public static void RegisterAutofacDisposeHandler(this IServiceProvider self) {
        var lifetime = Prevent.Argument
                              .Null(self)
                              .GetService<IHostApplicationLifetime>();

        if (lifetime is null) { return; }

        var container = self.GetAutofacRoot();
        lifetime.ApplicationStopped.Register(container.Dispose);
    }
}