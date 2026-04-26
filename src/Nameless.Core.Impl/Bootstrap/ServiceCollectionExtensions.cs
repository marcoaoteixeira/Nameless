using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Helpers;

namespace Nameless.Bootstrap;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods.
/// </summary>
public static class ServiceCollectionExtensions {
    extension(IServiceCollection self) {
        public IServiceCollection RegisterBootstrap(Action<BootstrapRegistration>? registration = null, IConfiguration? configuration = null) {
            var settings = ActionHelper.FromDelegate(registration);

            self.ConfigureOptions<BootstrapOptions>(configuration);

            self.TryAddTransient<Bootstrapper>();
            self.TryAddTransient<ParallelBootstrapper>();
            self.TryAddTransient<IBootstrapper>(ResolveBootstrapper);

            self.TryAddEnumerable(
                descriptors: CreateStepServiceDescriptors(settings)
            );

            return self;
        }
    }

    private static Bootstrapper ResolveBootstrapper(IServiceProvider provider) {
        var options = provider.GetOptions<BootstrapOptions>();

        return options.Value.EnableParallelExecution
            ? provider.GetRequiredService<ParallelBootstrapper>()
            : provider.GetRequiredService<Bootstrapper>();
    }

    private static IEnumerable<ServiceDescriptor> CreateStepServiceDescriptors(BootstrapRegistration settings) {
        var service = typeof(IStep);
        var implementations = settings.UseAssemblyScan
            ? settings.ExecuteAssemblyScan<IStep>()
            : settings.Steps;

        return implementations.Select(
            implementation => ServiceDescriptor.Transient(service, implementation)
        );
    }
}