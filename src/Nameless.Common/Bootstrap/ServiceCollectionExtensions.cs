using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Helpers;

namespace Nameless.Bootstrap;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods.
/// </summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions {
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers the bootstrapper and its steps in the service collection.
        /// </summary>
        /// <param name="registration">Optional delegate to register steps.</param>
        /// <param name="configuration">Optional configuration for <see cref="BootstrapOptions"/>.</param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> so other actions can be chained.
        /// </returns>
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