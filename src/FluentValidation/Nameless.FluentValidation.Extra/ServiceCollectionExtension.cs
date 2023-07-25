using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.FluentValidation {
    public static class ServiceCollectionExtension {
        #region Public Static Methods

        public static IServiceCollection ConfigureFluentValidation(this IServiceCollection services, params Assembly[] assemblies)
            => services.ConfigureFluentValidation(assemblies);

        #endregion
    }
}
