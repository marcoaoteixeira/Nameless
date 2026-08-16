using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Helpers;

namespace Nameless.Web.ErrorHandling;

public static class ServiceCollectionExtensions {
    extension(IServiceCollection self) {
        public IServiceCollection RegisterExceptionHandlers(Action<ExceptionHandlerRegistration>? registration = null, bool useGlobalExceptionHandler = true) {
            if (useGlobalExceptionHandler) {
                self.AddExceptionHandler<GlobalExceptionHandler>();
            }

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
