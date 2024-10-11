using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Nameless.Autofac;

/// <summary>
/// <see cref="IServiceProvider"/> extensions methods.
/// </summary>
public static class ServiceProviderExtension {
    /// <summary>
    /// Adds Autofac dispose action to the application lifecycle stop event.
    /// </summary>
    /// <param name="self">The current <see cref="IServiceProvider"/> instance.</param>
    public static void UseAutofacDestroyRoutine(this IServiceProvider self) {
        Prevent.Argument.Null(self);

        // Tear down the composition root and free all resources
        // when the application stops.
        var container = self.GetAutofacRoot();
        var lifetime = self.GetRequiredService<IHostApplicationLifetime>();

        lifetime.ApplicationStopped.Register(container.Dispose);
    }
}