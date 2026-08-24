using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Helpers;

namespace Nameless.Web.ErrorHandling;

public static class ServiceCollectionExtensions {
    extension(IServiceCollection self) {
        public IServiceCollection RegisterErrorHandling(Action<ErrorHandlingRegistration>? registration = null) {
            var settings = ActionHelper.FromDelegate(registration);
            var implementations = settings.UseAssemblyScan
                ? settings.ExecuteAssemblyScan<IExceptionHandler>()
                : settings.ExceptionHandlers;
            var descriptions = implementations.Select(
                implementation => ServiceDescriptor.Singleton(typeof(IExceptionHandler), implementation)
            );

            self.TryAdd(descriptions);

            return self;
        }
    }
}
