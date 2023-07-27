using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.FluentValidation {
    public static class ServiceCollectionExtension {
        #region Public Static Methods

        public static IServiceCollection RegisterFluentValidation(this IServiceCollection services, params Assembly[] assemblies)
            => services
                .AddSingleton<IValidationService, ValidationService>()
                .AddValidatorsFromAssemblies(assemblies);

        #endregion
    }
}
