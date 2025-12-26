using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
        /// <param name="configure">
        ///     The configuration action.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection" /> so other actions
        ///     can be chained.
        /// </returns>
        public IServiceCollection RegisterValidation(Action<ValidationOptions>? configure = null) {
            var innerConfigure = configure ?? (_ => { });
            var options = new ValidationOptions();

            innerConfigure(options);

            self.RegisterValidators(options);

            self.TryAddTransient<IValidationService, ValidationService>();

            return self;
        }

        private void RegisterValidators(ValidationOptions options) {
            var service = typeof(IValidator);
            var implementations = options.Assemblies
                                         .GetImplementations(service)
                                         .Where(type => !type.IsGenericTypeDefinition);

            // FluentValidation validators can be implemented using one
            // of these two interfaces or abstract class:
            // - AbstractValidator<T> (most common)
            // - IValidator<T> (more or less common)
            // - IValidator (less common)

            // We would like to register all of them, so when we try
            // to resolve a validator for a specific type, it will be
            // available in the DI container.
            var similar = new List<ServiceDescriptor>();

            foreach (var implementation in implementations) {
                // We're going to add to the similar list all the
                // implementations with the IValidator interface,
                // so we can resolve all of them at once with
                // the GetServices<IValidator>() method.
                similar.Add(ServiceDescriptor.Transient(service, implementation));

                // Now, register for the concrete types.
                self.TryAddTransient(implementation, implementation);

                // Register for the generic IValidator<T> interface
                foreach (var interfaceImpl in implementation.GetInterfacesThatCloses(typeof(IValidator<>))) {
                    self.TryAddTransient(interfaceImpl, implementation);
                }

                // Register for the generic AbstractValidator<T> interface
                foreach (var abstractImpl in implementation.GetInterfacesThatCloses(typeof(AbstractValidator<>))) {
                    self.TryAddTransient(abstractImpl, implementation);
                }
            }

            // Finally, register all the IValidator implementations
            self.TryAddEnumerable(similar);
        }
    }
}