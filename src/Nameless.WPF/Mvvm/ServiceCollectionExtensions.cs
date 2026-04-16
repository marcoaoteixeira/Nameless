using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Helpers;
using Nameless.WPF.DependencyInjection;

namespace Nameless.WPF.Mvvm;

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
        /// <param name="registration">
        ///     The registration settings delegate.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> so other actions
        ///     can be chained.
        /// </returns>
        public IServiceCollection RegisterViewModels(Action<ViewModelRegistrationSettings> registration) {
            var settings = ActionHelper.FromDelegate(registration);

            self.TryAdd(
                descriptors: CreateViewModelServiceDescriptors(settings)
            );

            return self;
        }
    }

    private static IEnumerable<ServiceDescriptor> CreateViewModelServiceDescriptors(ViewModelRegistrationSettings settings) {
        var service = typeof(ViewModel);
        var implementations = settings.UseAssemblyScan
            ? settings.ExecuteAssemblyScan(service)
            : settings.ViewModels;

        return implementations.Select(
            implementation => implementation.CreateServiceDescriptor()
        );
    }
}