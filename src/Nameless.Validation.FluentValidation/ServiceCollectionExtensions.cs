using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Nameless.Validation.FluentValidation;

/// <summary>
///     Extension methods for <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <summary>
    ///     Registers the validation services.
    /// </summary>
    /// <param name="self">The current <see cref="IServiceCollection" />.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>
    ///     The current <see cref="IServiceCollection" /> so that additional calls can be chained.
    /// </returns>
    public static IServiceCollection RegisterValidation(this IServiceCollection self, Action<ValidationOptions>? configure = null) {
        var innerConfigure = configure ?? (_ => { });
        var options = new ValidationOptions();

        innerConfigure(options);

        return self.RegisterValidators(options)
                   .RegisterMainServices();
    }

    private static IServiceCollection RegisterValidators(this IServiceCollection self, ValidationOptions options) {
        var service = typeof(IValidator);
        var descriptors = options.Assemblies
                                 .GetImplementations(service)
                                 .Where(type => !type.IsGenericTypeDefinition)
                                 .Select(validator => new ServiceDescriptor(service, validator, ServiceLifetime.Singleton));

        self.TryAddEnumerable(descriptors);

        return self;
    }

    private static IServiceCollection RegisterMainServices(this IServiceCollection self) {
        self.TryAddTransient<IValidationService, ValidationService>();

        return self;
    }
}