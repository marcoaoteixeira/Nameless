using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Bootstrap.Infrastructure;
using Nameless.Bootstrap.Resilience;
using Nameless.Helpers;
using Nameless.Registration;

namespace Nameless.Bootstrap;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods.
/// </summary>
public static class ServiceCollectionExtensions {
    private const string SEQUENTIAL_BOOTSTRAPPER_KEY = $"{nameof(Bootstrapper)} :: 04118d74-41f2-48fd-8761-42af0ddcf0a7";
    private const string PARALLEL_BOOTSTRAPPER_KEY = $"{nameof(ParallelBootstrapper)} :: 791d0bc2-d1e4-4140-b722-5fcade174486";

    extension(IServiceCollection self) {
        public IServiceCollection RegisterBootstrap(Action<BootstrapRegistrationSettings> registration) {
            return self.Configure<BootstrapOptions>(_ => { })
                       .InnerRegisterBootstrap(registration);
        }

        public IServiceCollection RegisterBootstrap(Action<BootstrapRegistrationSettings> registration, IConfiguration configuration) {
            return self.Configure<BootstrapOptions>(configuration)
                       .InnerRegisterBootstrap(registration);
        }

        /// <summary>
        ///     Registers the bootstrapping services and steps with the
        ///     current <see cref="IServiceCollection"/> instance.
        /// </summary>
        /// <param name="registration">
        ///     The registration settings delegate.
        /// </param>
        /// <param name="configure">
        ///     The configuration delegate.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public IServiceCollection RegisterBootstrap(Action<BootstrapRegistrationSettings> registration, Action<BootstrapOptions> configure) {
            return self.Configure(configure)
                       .InnerRegisterBootstrap(registration);
        }

        private IServiceCollection InnerRegisterBootstrap(Action<BootstrapRegistrationSettings> registration) {
            var settings = ActionHelper.FromDelegate(registration);

            self.TryAddKeyedTransient<Bootstrapper>(SEQUENTIAL_BOOTSTRAPPER_KEY);
            self.TryAddKeyedTransient<ParallelBootstrapper>(PARALLEL_BOOTSTRAPPER_KEY);
            self.TryAddTransient<IRetryPolicyFactory, RetryPolicyFactory>();
            self.TryAddTransient<IBootstrapper>(ResolveBootstrapper);
            
            self.TryAddEnumerable(
                descriptors: CreateStepDescriptors(settings)
            );

            return self;
        }
    }

    private static Bootstrapper ResolveBootstrapper(IServiceProvider provider) {
        var options = provider.GetOptions<BootstrapOptions>();

        return options.Value.EnableParallelExecution
            ? provider.GetRequiredKeyedService<ParallelBootstrapper>(PARALLEL_BOOTSTRAPPER_KEY)
            : provider.GetRequiredKeyedService<Bootstrapper>(SEQUENTIAL_BOOTSTRAPPER_KEY);
    }

    private static IEnumerable<ServiceDescriptor> CreateStepDescriptors(BootstrapRegistrationSettings settings) {
        var service = typeof(IStep);
        var implementations = settings.UseAssemblyScan
            ? settings.GetImplementationsFor(service)
            : settings.Steps;

        return implementations.Select(implementation => ServiceDescriptor.Transient(
            service,
            implementation
        ));
    }
}