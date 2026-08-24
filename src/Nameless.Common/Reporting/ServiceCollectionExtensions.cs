using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Helpers;

namespace Nameless.Reporting;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods that register
///     the <see cref="IStatusReporter{TService}"/> services.
/// </summary>
public static class ServiceCollectionExtensions {
    // Not a security boundary - just keeps the concrete
    // StatusReporterHub<TService> out of reach of plain constructor injection
    // or GetService<T>() call, so callers are steered toward
    // IStatusReporterHub<TService> / IStatusMonitorHub<TService>
    // (or IStatusReporter<TService> / IStatusMonitor<TService> for the
    // single-channel case).
    private const string CONCRETE_HUB_KEY = "::INTERNAL::STATUS::REPORTER::HUB";

    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/> instance.
    /// </param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers <see cref="StatusReporterHub"/> for all
        ///     services defined by the <see cref="StatusReportingRegistration"/>
        ///     delegate.
        /// </summary>
        /// <param name="registration">
        ///     The registration delegate.
        /// </param>
        /// <returns>
        ///     The current instance of <see cref="IServiceCollection"/> so other
        ///     actions can be chained.
        /// </returns>
        public IServiceCollection RegisterStatusReporting(Action<StatusReportingRegistration>? registration = null) {
            var settings = ActionHelper.FromDelegate(registration);

            self.TryAddKeyedSingleton<StatusReporterHub>(
                CONCRETE_HUB_KEY, (_, _) => new StatusReporterHub(settings.BufferSize)
            );

            self.TryAddSingleton<IStatusReporterHub>(
                provider => provider.GetRequiredKeyedService<StatusReporterHub>(CONCRETE_HUB_KEY)
            );

            self.TryAddSingleton<IStatusMonitorHub>(
                provider => provider.GetRequiredKeyedService<StatusReporterHub>(CONCRETE_HUB_KEY)
            );

            var services = settings.UseAssemblyScan
                ? Scan(settings.Assemblies)
                : settings.ForServices;

            foreach (var service in services) {
                self.RegisterStatusReportingForService(service);
            }

            return self;
        }

        private void RegisterStatusReportingForService(Type service) {
            Throws.When.IsNonConcreteType(service);
            Throws.When.IsOpenGenericType(service);

            var statusReporterService = typeof(IStatusReporter<>).MakeGenericType(service);
            self.TryAddSingleton(
                service: statusReporterService,
                implementationFactory: provider => CreateStatusReporterInstance(service, provider)
            );

            var statusMonitorService = typeof(IStatusMonitor<>).MakeGenericType(service);
            self.TryAddSingleton(
                service: statusMonitorService,
                implementationFactory: provider => CreateStatusReporterInstance(service, provider)
            );
        }
    }

    private static object CreateStatusReporterInstance(Type service, IServiceProvider provider) {
        var statusReporterHub = provider.GetRequiredService<IStatusReporterHub>();
        var handler = typeof(IStatusReporterHub).GetMethod(nameof(IStatusReporterHub.GetOrCreate))
                      ?? throw new InvalidOperationException($"Missing '{nameof(IStatusReporterHub.GetOrCreate)}' method.");

        return handler.MakeGenericMethod(service)
                      .Invoke(statusReporterHub, parameters: [string.Empty])
               ?? throw new InvalidOperationException("Unable to initialize Status Monitor/Reporter.");
    }

    private static IEnumerable<Type> Scan(IEnumerable<Assembly> assemblies) {
        return assemblies.SelectMany(assembly => assembly.GetExportedTypes())
                         .Where(type => type.HasAttribute<StatusReportingAttribute>());
    }
}