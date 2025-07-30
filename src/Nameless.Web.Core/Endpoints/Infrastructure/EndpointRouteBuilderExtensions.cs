using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Web.Endpoints.Definitions;

namespace Nameless.Web.Endpoints.Infrastructure;

/// <summary>
///     Extension methods for <see cref="IEndpointRouteBuilder"/>.
/// </summary>
internal static class EndpointRouteBuilderExtensions {
    /// <summary>
    ///     Maps an endpoint using the provided
    ///     <see cref="IEndpointDescriptor"/>.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="IEndpointRouteBuilder"/> instance.
    /// </param>
    /// <param name="descriptor">
    ///     The endpoint descriptor.
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///     if the endpoint route pattern is not defined or
    ///     if the anti-forgery middleware is not available.
    /// </exception>
    internal static void MapEndpoint(this IEndpointRouteBuilder self, IEndpointDescriptor descriptor) {
        var routeHandlerBuilder = self.MapMethods(
            pattern: descriptor.RoutePattern,
            httpMethods: [descriptor.HttpMethod],
            handler: (HttpContext httpContext, [FromServices] IEndpointFactory factory) => EndpointInvoker.InvokeAsync(httpContext, factory)
        );

        routeHandlerBuilder.WithName(descriptor.GetName());

        if (!string.IsNullOrWhiteSpace(descriptor.DisplayName)) {
            routeHandlerBuilder.WithDisplayName(descriptor.DisplayName);
        }

        if (!string.IsNullOrWhiteSpace(descriptor.Description)) {
            routeHandlerBuilder.WithDescription(descriptor.Description);
        }

        if (!string.IsNullOrWhiteSpace(descriptor.Summary)) {
            routeHandlerBuilder.WithSummary(descriptor.Summary);
        }

        routeHandlerBuilder.WithTags(descriptor.Tags);

        routeHandlerBuilder.MapToApiVersion(descriptor.Version.Number);

        routeHandlerBuilder.WithMetadata(descriptor.Version);

        if (!string.IsNullOrWhiteSpace(descriptor.RequestTimeoutPolicy)) {
            routeHandlerBuilder.WithRequestTimeout(descriptor.RequestTimeoutPolicy);
        }

        if (!string.IsNullOrWhiteSpace(descriptor.RateLimitingPolicy)) {
            routeHandlerBuilder.RequireRateLimiting(descriptor.RateLimitingPolicy);
        }

        routeHandlerBuilder.RequireAuthorization([.. descriptor.AuthorizationPolicies]);

        if (!string.IsNullOrWhiteSpace(descriptor.CorsPolicy)) {
            routeHandlerBuilder.RequireCors(descriptor.CorsPolicy);
        }

        if (!string.IsNullOrWhiteSpace(descriptor.OutputCachePolicy)) {
            routeHandlerBuilder.CacheOutput(descriptor.OutputCachePolicy);
        }

        if (descriptor.AllowAnonymous) {
            routeHandlerBuilder.AllowAnonymous();
        }

        switch (descriptor.UseAntiforgery) {
            case true when self.ServiceProvider.GetService<IAntiforgery>() is null:
                throw new InvalidOperationException("Anti-forgery middleware not available.");
            case false:
                routeHandlerBuilder.DisableAntiforgery();
                break;
        }

        if (descriptor.DisableHttpMetrics) {
            routeHandlerBuilder.DisableHttpMetrics();
        }

        foreach (var accepts in descriptor.Accepts) {
            routeHandlerBuilder.WithMetadata(accepts);
        }

        foreach (var produces in descriptor.Produces) {
            routeHandlerBuilder.WithMetadata(produces);
        }

        var endpointFilterBuilder = new EndpointFilterBuilder(routeHandlerBuilder);
        foreach (var filter in descriptor.Filters) {
            filter(endpointFilterBuilder);
        }

        routeHandlerBuilder.WithEndpointDescriptorMetadata(descriptor);
    }
}
