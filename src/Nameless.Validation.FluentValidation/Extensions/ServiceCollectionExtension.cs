using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Validation.FluentValidation.Impl;

namespace Nameless.Validation.FluentValidation;

/// <summary>
/// <see cref="IServiceCollection"/> extension methods.
/// </summary>
public static class ServiceCollectionExtension {
    /// <summary>
    /// Adds the validation service to the service collection.
    /// </summary>
    /// <param name="self">The <see cref="IServiceCollection"/> current instance.</param>
    /// <param name="supportAssemblies">The assemblies that will be scanned for <see cref="AbstractValidator{T}"/> implementations.</param>
    /// <returns>
    /// The <see cref="IServiceCollection"/> current instance so other actions can be chained.
    /// </returns>
    public static IServiceCollection AddValidationService(this IServiceCollection self, params Assembly[] supportAssemblies) {
        // Scan and register validators.
        AssemblyScanner.FindValidatorsInAssemblies(supportAssemblies)
                       .ForEach(result => self.AddSingleton(serviceType: result.InterfaceType,
                                                            implementationType: result.ValidatorType));

        return self.AddSingleton<IValidationService, ValidationService>();
    }
}