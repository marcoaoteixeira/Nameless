using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Helpers;

namespace Nameless.Windows.Mvvm;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/>.
    /// </param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers all view models.
        /// </summary>
        /// <param name="configure">
        ///     The registration settings delegate.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> so other actions
        ///     can be chained.
        /// </returns>
        public IServiceCollection RegisterViewModels(Action<ViewModelRegistration> configure) {
            var registration = ActionHelper.FromDelegate(configure);

            var service = typeof(ViewModel);
            var implementations = registration.UseAssemblyScan
                ? registration.ExecuteAssemblyScan(service)
                : registration.ViewModels;

            var descriptors = implementations.Select(
                implementation => ServiceDescriptor.Transient(implementation, implementation)
            );

            self.TryAdd(descriptors);

            return self;
        }
    }
}