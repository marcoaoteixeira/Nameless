using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Validation.Abstractions;
using Nameless.Validation.FluentValidation.Impl;

namespace Nameless.Validation.FluentValidation;

public static class ServiceCollectionExtension {
    #region Public Static Methods

    public static IServiceCollection RegisterValidationService(this IServiceCollection self, params Assembly[] supportAssemblies) {
        // Scan and register validators.
        AssemblyScanner.FindValidatorsInAssemblies(supportAssemblies)
                       .ForEach(result => self.AddSingleton(serviceType: result.InterfaceType,
                                                            implementationType: result.ValidatorType));

        return self.AddSingleton<IValidationService>(provider => new ValidationService(validators: provider.GetServices<IValidator>(),
                                                                                       logger: provider.GetLogger<ValidationService>()));
    }

    #endregion
}