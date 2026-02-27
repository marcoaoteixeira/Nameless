using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Web.MinimalEndpoints.Definitions;

namespace Nameless.Web.MinimalEndpoints.Infrastructure;

/// <summary>
///     Extension methods for <see cref="IEndpointRouteBuilder"/>.
/// </summary>
internal static class EndpointRouteBuilderExtensions {
    /// <param name="self">
    ///     The current <see cref="IEndpointRouteBuilder"/> instance.
    /// </param>
    extension(IEndpointRouteBuilder self) {
        /// <summary>
        ///     Maps an endpoint using the provided
        ///     <see cref="IEndpointDescriptor"/>.
        /// </summary>
        /// <param name="descriptor">
        ///     The endpoint descriptor.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     if the endpoint route pattern is not defined or
        ///     if the anti-forgery middleware is not available.
        /// </exception>
        internal void MapEndpoint(IEndpointDescriptor descriptor) {
            var routeHandlerBuilder = self.MapMethods(
                descriptor.RoutePattern,
                [descriptor.HttpMethod],
                (HttpContext httpContext, [FromServices] IEndpointFactory factory) =>
                    EndpointInvoker.InvokeAsync(httpContext, factory)
            );

            routeHandlerBuilder.WithName(descriptor.GetNameOrDefault());

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

            switch (descriptor.AllowAnonymous) {
                case true:
                    routeHandlerBuilder.AllowAnonymous();
                    break;
                case false:
                    routeHandlerBuilder.RequireAuthorization([.. descriptor.AuthorizationPolicies]);
                    break;
            }

            if (!string.IsNullOrWhiteSpace(descriptor.CorsPolicy)) {
                routeHandlerBuilder.RequireCors(descriptor.CorsPolicy);
            }

            if (!string.IsNullOrWhiteSpace(descriptor.OutputCachePolicy)) {
                routeHandlerBuilder.CacheOutput(descriptor.OutputCachePolicy);
            }

            switch (descriptor.UseAntiforgery) {
                case true when self.ServiceProvider.GetService<IAntiforgery>() is null:
                    throw new InvalidOperationException(message: "Anti-forgery middleware not available.");
                case false:
                    routeHandlerBuilder.DisableAntiforgery();
                    break;
            }

            if (descriptor.DisableHttpMetrics) {
                routeHandlerBuilder.DisableHttpMetrics();
            }

            foreach (var accept in descriptor.Accepts) {
                routeHandlerBuilder.WithMetadata(new AcceptsMetadata(
                    accept.ContentTypes,
                    accept.RequestType,
                    accept.IsOptional
                ));
            }

            foreach (var produce in descriptor.Produces) {
                routeHandlerBuilder.WithMetadata(new ProducesResponseTypeMetadata(
                    produce.StatusCode,
                    produce.ResponseType,
                    produce.ContentTypes
                ));
            }

            var endpointFilterBuilder = new EndpointFilterBuilder(routeHandlerBuilder);
            foreach (var filter in descriptor.Filters) {
                filter(endpointFilterBuilder);
            }

            foreach (var additionalMetadata in descriptor.AdditionalMetadata) {
                routeHandlerBuilder.WithMetadata(additionalMetadata);
            }
        }
    }
}