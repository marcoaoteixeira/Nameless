using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Nameless.Autofac;

public static class ServiceProviderExtension {
    public static void UseAutofacDestroyRoutine(this IServiceProvider self) {
        Prevent.Argument.Null(self);

        // Tear down the composition root and free all resources
        // when the application stops.
        var container = self.GetAutofacRoot();
        var lifetime = self.GetRequiredService<IHostApplicationLifetime>();

        lifetime.ApplicationStopped.Register(container.Dispose);
    }
}