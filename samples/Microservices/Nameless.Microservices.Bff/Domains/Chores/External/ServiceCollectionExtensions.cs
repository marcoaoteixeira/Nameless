using Microsoft.AspNetCore.Authentication.JwtBearer;
using Nameless.Web.Auth;
using Refit;

namespace Nameless.Microservices.Bff.Domains.Chores.External;

public static class ServiceCollectionExtensions {
    extension(IServiceCollection self) {
        public IServiceCollection RegisterChoresHttpClient() {
            self.AddRefitClient<IChoresHttpClient>(ConfigureRefitSettings)
                .ConfigureHttpClient(client => client.BaseAddress = new Uri("https+http://api"))
                .AddHttpMessageHandler<AuthorizationForwardingHandler>();

            return self;
        }
    }

    // Gets the current request Authorization and pass it to the next
    // call to the API
    private static RefitSettings ConfigureRefitSettings(IServiceProvider _) {
        return new RefitSettings {
            AuthorizationHeaderValueGetter = (request, _) => {
                var scheme = request.Headers.Authorization?.Scheme;

                return scheme == JwtBearerDefaults.AuthenticationScheme
                    ? Task.FromResult(request.Headers.Authorization?.Parameter ?? string.Empty)
                    : Task.FromResult(string.Empty);
            }
        };
    }
}
