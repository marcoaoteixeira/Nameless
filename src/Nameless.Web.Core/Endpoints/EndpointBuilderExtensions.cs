using System.Reflection;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Web.Endpoints;

internal static class EndpointBuilderExtensions {
    internal static void Apply(this EndpointBuilder self, IEndpointRouteBuilder builder) {
        if (string.IsNullOrWhiteSpace(self.RoutePattern)) {
            builder.ServiceProvider
                   .GetLogger<EndpointBuilder>()
                   .MissingEndpointRoute(self.EndpointType);

            return;
        }

        var routeHandlerBuilder = builder.MapMethods(
            pattern: self.RoutePattern,
            httpMethods: [self.HttpMethod],
            handler: Prevent.Argument.Null(self.Handler));

        routeHandlerBuilder.WithName(self.GetName());

        if (self.DisplayName is not null) {
            routeHandlerBuilder.WithDisplayName(self.DisplayName);
        }

        if (self.Description is not null) {
            routeHandlerBuilder.WithDescription(self.Description);
        }

        if (self.Summary is not null) {
            routeHandlerBuilder.WithSummary(self.Summary);
        }

        routeHandlerBuilder.WithTags(self.Tags);

        if (self.RequestTimeoutPolicy is not null) {
            routeHandlerBuilder.WithRequestTimeout(self.RequestTimeoutPolicy);
        }

        foreach (var acceptMetadata in self.AcceptMetadataCollection) {
            routeHandlerBuilder.Accepts(
                acceptMetadata.RequestType,
                acceptMetadata.IsOptional,
                acceptMetadata.ContentType,
                acceptMetadata.AdditionalContentTypes);
        }

        foreach (var produceMetadata in self.ProducesMetadataCollection) {
            switch (produceMetadata.Type) {
                case EndpointBuilder.ProduceType.Result:
                    routeHandlerBuilder.Produces(
                        produceMetadata.StatusCode,
                        produceMetadata.ResponseType,
                        produceMetadata.ContentType,
                        produceMetadata.AdditionalContentTypes);
                    break;
                case EndpointBuilder.ProduceType.Problem:
                    routeHandlerBuilder.ProducesProblem(
                        produceMetadata.StatusCode,
                        produceMetadata.ContentType);
                    break;
                case EndpointBuilder.ProduceType.ValidationProblem:
                    routeHandlerBuilder.ProducesValidationProblem(
                        produceMetadata.StatusCode,
                        produceMetadata.ContentType);
                    break;
            }
        }

        foreach (var filterType in self.Filters) {
            routeHandlerBuilder.InvokeAddEndpointFilter(filterType);
        }

        if (self.UseAntiForgery && builder.ServiceProvider.GetService<IAntiforgery>() is null) {
            throw new InvalidOperationException("Anti-forgery middleware not available.");
        }

        foreach (var rateLimitingPolicy in self.RateLimitingPolicies) {
            routeHandlerBuilder.RequireRateLimiting(rateLimitingPolicy);
        }

        if (self.UseAllowAnonymous) {
            routeHandlerBuilder.AllowAnonymous();
        }

        routeHandlerBuilder.RequireAuthorization([.. self.AuthorizationPolicies]);

        foreach (var corsPolicy in self.CorsPolicies) {
            routeHandlerBuilder.RequireCors(corsPolicy);
        }

        routeHandlerBuilder.MapToApiVersion(self.Version);
        routeHandlerBuilder.WithMetadata(self.Stability);
    }
}

// !HACK! This code is an extension to the RouteHandlerBuilder to allow
// adding endpoint filters dynamically. This is necessary because the
// AddEndpointFilter method is generic and requires a specific type to
// be passed.
internal static class RouteHandlerBuilderAccessor {
    internal static void InvokeAddEndpointFilter(this RouteHandlerBuilder builder, Type filterType) {
        var handler = typeof(EndpointFilterExtensions)
                     .GetMethods(BindingFlags.Static | BindingFlags.Public)
                     .FirstOrDefault(member => member is { Name: nameof(EndpointFilterExtensions.AddEndpointFilter), IsGenericMethodDefinition: true, IsGenericMethod: true } &&
                                               member.GetGenericArguments().Length == 2 && // TBuilder & TFilterType
                                               member.GetParameters().Length == 1 && // current TBuilder
                                               typeof(IEndpointConventionBuilder).IsAssignableFrom(member.GetParameters()[0].ParameterType));
        if (handler is null) {
            throw new InvalidOperationException($"Could not find {nameof(EndpointFilterExtensions.AddEndpointFilter)} method in {nameof(EndpointFilterExtensions)}.");
        }

        handler.MakeGenericMethod(typeof(RouteHandlerBuilder), filterType).Invoke(null, [builder]);
    }
}
