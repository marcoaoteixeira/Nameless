using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Helpers;

namespace Nameless.Validation.FluentValidation;

/// <summary>
///     <see cref="IServiceCollection" /> extension methods.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="self">
    ///     The current <see cref="IServiceCollection" />.
    /// </param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers the validation services.
        /// </summary>
        /// <param name="registration">
        ///     The registration settings delegate.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection" /> so other actions
        ///     can be chained.
        /// </returns>
        public IServiceCollection RegisterValidation(Action<ValidationRegistration>? registration = null) {
            var settings = ActionHelper.FromDelegate(registration);

            self.TryAddEnumerable(
                descriptors: CreateValidatorServiceDescriptors(settings)
            );

            self.TryAddTransient<IValidationService, ValidationService>();

            return self;
        }
    }

    private static IEnumerable<ServiceDescriptor> CreateValidatorServiceDescriptors(ValidationRegistration settings) {
        var service = typeof(IValidator);
        var implementations = settings.UseAssemblyScan
            ? settings.ExecuteAssemblyScan(service)
            : settings.Validators;

        return implementations.Select(
            implementation => ServiceDescriptor.Transient(service, implementation)
        );
    }
}