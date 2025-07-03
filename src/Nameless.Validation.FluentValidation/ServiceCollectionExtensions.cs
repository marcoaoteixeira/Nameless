using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

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

        return self
            .RegisterValidators(options)
            .RegisterMainServices();
    }

    private static IServiceCollection RegisterValidators(this IServiceCollection self, ValidationOptions options) {
        var serviceType = typeof(IValidator);
        var validators = options.Assemblies
                                .GetImplementations([serviceType])
                                .Where(type => !type.IsGenericTypeDefinition);

        foreach (var validator in validators) {
            self.AddScoped(serviceType, validator);
        }

        return self;
    }

    private static IServiceCollection RegisterMainServices(this IServiceCollection self) {
        return self.AddScoped<IValidationService, ValidationService>();
    }
}