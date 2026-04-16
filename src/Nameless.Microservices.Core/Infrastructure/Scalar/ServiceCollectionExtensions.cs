using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Helpers;

namespace Nameless.Microservices.Infrastructure.Scalar;

public static class ServiceCollectionExtensions {
    extension(IServiceCollection self) {
        public IServiceCollection RegisterScalar(Action<ScalarRegistrationSettings>? registration = null) {
            var settings = ActionHelper.FromDelegate(registration);

            var service = typeof(IAuthorizationHttpClient);
            var implementation = settings.UseAssemblyScan
                ? settings.ExecuteAssemblyScan(service).SingleOrDefault()
                : settings.AuthorizationHttpClient;

            if (implementation is null) {
                throw new InvalidOperationException($"Implementation for '{nameof(IAuthorizationHttpClient)}' was not found.");
            }

            self.AddHttpClient();
            self.TryAddTransient(service, implementation);

            return self;
        }
    }
}