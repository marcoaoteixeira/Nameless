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
        public IServiceCollection RegisterValidator(Action<ValidatorRegistration>? registration = null) {
            var settings = ActionHelper.FromDelegate(registration);

            self.TryAddTransient<IValidator, FluentValidationValidator>();
            self.RegisterValidators(settings);

            return self;
        }

        private void RegisterValidators(ValidatorRegistration settings) {
            var service = typeof(IFluentValidator);
            var implementations = settings.UseAssemblyScan
                ? settings.ExecuteAssemblyScan(service)
                : settings.Validators;
            var descriptors = implementations.Select(
                implementation => ServiceDescriptor.Transient(service, implementation)
            );

            self.TryAddEnumerable(descriptors);
        }
    }
}