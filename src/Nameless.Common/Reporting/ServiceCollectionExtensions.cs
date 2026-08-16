using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Nameless.Reporting;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods that register
///     the <see cref="IStatusReporter{TService}"/> services.
/// </summary>
public static class ServiceCollectionExtensions {
    // Not a security boundary - just keeps the concrete
    // StatusReporter<TService> out of reach of plain constructor injection
    // or GetService<T>() call, so callers are steered toward
    // IStatusReporter<TService> / IStatusMonitor<TService>.
    private const string CONCRETE_REPORTER_KEY = "Reporting::internal::StatusReporter";

    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/> instance.
    /// </param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers a <see cref="StatusReporter{TService}"/> singleton for
        ///     <typeparamref name="TService"/>, exposed only via
        ///     <see cref="IStatusReporter{TService}"/> (write+read) and
        ///     <see cref="IStatusMonitor{TService}"/> (read-only) - both resolve
        ///     to the same underlying instance. Safe to call more than once for
        ///     the same <typeparamref name="TService"/>.
        /// </summary>
        public IServiceCollection RegisterStatusReporter<TService>() {
            self.TryAddKeyedSingleton<StatusReporter<TService>>(CONCRETE_REPORTER_KEY);

            self.TryAddSingleton<IStatusReporter<TService>>(
                provider => provider.GetRequiredKeyedService<StatusReporter<TService>>(CONCRETE_REPORTER_KEY)
            );

            self.TryAddSingleton<IStatusMonitor<TService>>(
                provider => provider.GetRequiredKeyedService<StatusReporter<TService>>(CONCRETE_REPORTER_KEY)
            );

            return self;
        }
    }
}