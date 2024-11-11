using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nameless.Web.Endpoints;

namespace Nameless.Web;

/// <summary>
///     <see cref="IEndpointRouteBuilder" /> extension methods.
/// </summary>
internal static class EndpointRouteBuilderExtension {
    /// <summary>
    ///     Maps the endpoint.
    /// </summary>
    /// <param name="self">The <see cref="IEndpointRouteBuilder" /> current instance.</param>
    /// <param name="endpoint">The endpoint.</param>
    internal static void Map(this IEndpointRouteBuilder self, MinimalEndpointBase endpoint) {
        var logger = self.ServiceProvider.GetLogger(typeof(EndpointRouteBuilderExtension));

        if (string.IsNullOrWhiteSpace(endpoint.RoutePattern)) {
            logger.EndpointMissingRoutePatternWarning(endpoint);

            return;
        }

        RouteHandlerBuilder? routeHandlerBuilder;

        try {
            routeHandlerBuilder = endpoint.HttpMethod switch {
                HttpMethods.DELETE => self.MapDelete(endpoint.RoutePattern, endpoint.CreateDelegate()),
                HttpMethods.GET => self.MapGet(endpoint.RoutePattern, endpoint.CreateDelegate()),
                HttpMethods.PATCH => self.MapPatch(endpoint.RoutePattern, endpoint.CreateDelegate()),
                HttpMethods.POST => self.MapPost(endpoint.RoutePattern, endpoint.CreateDelegate()),
                HttpMethods.PUT => self.MapPut(endpoint.RoutePattern, endpoint.CreateDelegate()),
                _ => null
            };
        } catch (Exception ex) {
            logger.EndpointMappingError(endpoint, ex);

            return;
        }

        if (routeHandlerBuilder is null) {
            logger.EndpointNotMappedWarning(endpoint);

            return;
        }

        var builder = new MinimalEndpointBuilder(routeHandlerBuilder, self);

        try { endpoint.Configure(builder); }
        catch (Exception ex) { logger.EndpointConfigurationError(endpoint, ex); }
    }
}