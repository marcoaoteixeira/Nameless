using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Web.Endpoints;
public static class ApplicationBuilderExtension {
    /// <summary>
    /// Configures API endpoints when implementing <see cref="EndpointBase"/> interface.
    /// A call to <see cref="UseMinimalEndpoints"/> must be preceded by a call to <see cref="EndpointRoutingApplicationBuilderExtensions.UseRouting"/>.
    /// </summary>
    /// <param name="self">The current <see cref="IApplicationBuilder"/> instance.</param>
    /// <returns>
    /// The current <see cref="IApplicationBuilder"/> instance so other actions can be chained.
    /// </returns>
    public static IApplicationBuilder UseMinimalEndpoints(this IApplicationBuilder self) {
        Prevent.Argument.Null(self);

        self.UseEndpoints(builder => {
            var logger = self.ApplicationServices
                             .GetLogger(typeof(ApplicationBuilderExtension));
            var endpoints = builder.ServiceProvider
                                   .GetServices<EndpointBase>();

            foreach (var endpoint in endpoints) {
                builder.Map(endpoint, logger);
            }
        });

        return self;
    }
}
