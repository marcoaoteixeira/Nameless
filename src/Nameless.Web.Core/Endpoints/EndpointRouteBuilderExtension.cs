using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Nameless.Web.Internals;

namespace Nameless.Web.Endpoints;
internal static class EndpointRouteBuilderExtension {
    internal static void Map(this IEndpointRouteBuilder self, IEndpoint endpoint, ILogger logger) {
        if (string.IsNullOrWhiteSpace(endpoint.RoutePattern)) {
            logger.EndpointMissingRoutePattern(endpoint);
            return;
        }

        var routeHandlerBuilder = endpoint.HttpMethod.ToUpperInvariant() switch {
            Root.HttpMethods.DELETE => self.MapDelete(endpoint.RoutePattern, endpoint.CreateDelegate()),
            Root.HttpMethods.GET => self.MapGet(endpoint.RoutePattern, endpoint.CreateDelegate()),
            Root.HttpMethods.PATCH => self.MapPatch(endpoint.RoutePattern, endpoint.CreateDelegate()),
            Root.HttpMethods.POST => self.MapPost(endpoint.RoutePattern, endpoint.CreateDelegate()),
            Root.HttpMethods.PUT => self.MapPut(endpoint.RoutePattern, endpoint.CreateDelegate()),
            _ => null
        };

        if (routeHandlerBuilder is null) {
            logger.EndpointNotMapped(endpoint);
            return;
        }

        routeHandlerBuilder.WithOpenApi();

        if (!string.IsNullOrWhiteSpace(endpoint.Name)) {
            routeHandlerBuilder.WithName(endpoint.Name);
        }

        if (!string.IsNullOrWhiteSpace(endpoint.Description)) {
            routeHandlerBuilder.WithDescription(endpoint.Description);
        }

        if (!string.IsNullOrWhiteSpace(endpoint.Summary)) {
            routeHandlerBuilder.WithSummary(endpoint.Summary);
        }

        if (!endpoint.Tags.IsNullOrEmpty()) {
            routeHandlerBuilder.WithTags(endpoint.Tags);
        }

        if (!endpoint.Accepts.IsNullOrEmpty()) {
            foreach (var acceptMetadata in endpoint.Accepts) {
                if (acceptMetadata.RequestType is null) {
                    continue;
                }

                routeHandlerBuilder.Accepts(acceptMetadata.RequestType,
                                            acceptMetadata.ContentType,
                                            acceptMetadata.AdditionalContentTypes);
            }
        }

        if (endpoint.Version > 0) {
            var apiVersion = new ApiVersion(endpoint.Version);
            var apiVersionSetBuilder = self.NewApiVersionSet(endpoint.GroupName)
                                           .HasApiVersion(apiVersion);

            if (endpoint.Deprecated) {
                apiVersionSetBuilder.HasDeprecatedApiVersion(apiVersion);
            }

            routeHandlerBuilder.WithApiVersionSet(apiVersionSetBuilder.Build());
        }
        
        if (endpoint.MapToVersion > 0) {
            routeHandlerBuilder.MapToApiVersion(endpoint.MapToVersion);
        }

        if (!endpoint.Produces.IsNullOrEmpty()) {
            foreach (var producesMetadata in endpoint.Produces) {
                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                switch (producesMetadata.Type) {
                    case ProducesResultType.Default:
                        routeHandlerBuilder.Produces((int)producesMetadata.StatusCode,
                                                     producesMetadata.ResponseType,
                                                     producesMetadata.ContentType,
                                                     producesMetadata.AdditionalContentTypes);
                        break;
                    case ProducesResultType.Problems:
                        routeHandlerBuilder.ProducesProblem((int)producesMetadata.StatusCode,
                                                            producesMetadata.ContentType);
                        break;
                    case ProducesResultType.ValidationProblems:
                        routeHandlerBuilder.ProducesValidationProblem(contentType: producesMetadata.ContentType);
                        break;
                }
            }
        }
    }
}
