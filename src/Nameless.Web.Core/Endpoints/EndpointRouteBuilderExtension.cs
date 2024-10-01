using System.Diagnostics.CodeAnalysis;
using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Nameless.Web.Filters;

namespace Nameless.Web.Endpoints;

/// <summary>
/// <see cref="IEndpointRouteBuilder"/> extension methods.
/// </summary>
internal static class EndpointRouteBuilderExtension {
    /// <summary>
    /// Maps the endpoint.
    /// </summary>
    /// <param name="self">The <see cref="IEndpointRouteBuilder"/> current instance.</param>
    /// <param name="endpoint">The endpoint.</param>
    internal static void Map(this IEndpointRouteBuilder self, EndpointBase endpoint) {
        var logger = self.ServiceProvider.GetLogger(typeof(EndpointRouteBuilderExtension));

        if (string.IsNullOrWhiteSpace(endpoint.RoutePattern)) {
            logger.EndpointMissingRoutePattern(endpoint);
            return;
        }

        if (!TryCreateRouteHandlerBuilder(self, endpoint, out var routeHandlerBuilder)) {
            logger.EndpointNotMapped(endpoint);
            return;
        }

        SetAcceptHeader(endpoint, routeHandlerBuilder, logger);
        SetOpenApiMetadata(endpoint, routeHandlerBuilder);
        SetVersioningInfo(endpoint, routeHandlerBuilder, endpointRouteBuilder: self);
        SetAuthorize(endpoint, routeHandlerBuilder);
        SetValidationFilter(endpoint, routeHandlerBuilder);
    }

    private static void SetValidationFilter(EndpointBase endpoint, RouteHandlerBuilder routeHandlerBuilder) {
        if (endpoint.UseValidationFilter) {
            routeHandlerBuilder.AddEndpointFilter<ValidateEndpointFilter>();
        }
    }

    private static void SetAuthorize(EndpointBase endpoint, RouteHandlerBuilder routeHandlerBuilder) {
        var authorizeData = endpoint.GetAuthorize()
                                    .ToArray();

        if (authorizeData.Length == 0) { return; }

        routeHandlerBuilder.RequireAuthorization(authorizeData);
    }

    private static void SetVersioningInfo(EndpointBase endpoint,
                                          RouteHandlerBuilder routeHandlerBuilder,
                                          IEndpointRouteBuilder endpointRouteBuilder) {
        var openApiMetadata = endpoint.GetOpenApiMetadata();
        var versioning = endpoint.GetVersioningInfo();

        if (string.IsNullOrWhiteSpace(openApiMetadata.GroupName) ||
            versioning is { Version: <= 0 }) { return; }

        var apiVersion = new ApiVersion(versioning.Version);
        var apiVersionSetBuilder = endpointRouteBuilder.NewApiVersionSet(openApiMetadata.GroupName)
                                                       .HasApiVersion(apiVersion);

        if (versioning.IsDeprecated) {
            apiVersionSetBuilder.HasDeprecatedApiVersion(apiVersion);
        }

        if (versioning.MapToVersion > 0) {
            routeHandlerBuilder.MapToApiVersion(versioning.MapToVersion);
        }

        routeHandlerBuilder.WithApiVersionSet(apiVersionSetBuilder.Build());
    }

    private static void SetOpenApiMetadata(EndpointBase endpoint, RouteHandlerBuilder routeHandlerBuilder) {
        var openApiMetadata = endpoint.GetOpenApiMetadata();
        if (string.IsNullOrWhiteSpace(openApiMetadata.Name)) {
            return;
        }

        routeHandlerBuilder.WithOpenApi()
                           .WithName(openApiMetadata.Name);

        if (!string.IsNullOrWhiteSpace(openApiMetadata.Description)) {
            routeHandlerBuilder.WithDescription(openApiMetadata.Description);
        }

        if (!string.IsNullOrWhiteSpace(openApiMetadata.Summary)) {
            routeHandlerBuilder.WithSummary(openApiMetadata.Summary);
        }

        if (!openApiMetadata.Tags.IsNullOrEmpty()) {
            routeHandlerBuilder.WithTags(openApiMetadata.Tags);
        }

        if (openApiMetadata.Produces.IsNullOrEmpty()) {
            return;
        }

        foreach (var producesMetadata in openApiMetadata.Produces) {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (producesMetadata.Type) {
                case ProducesType.Default:
                    routeHandlerBuilder.Produces((int)producesMetadata.StatusCode,
                                                 producesMetadata.ResponseType,
                                                 producesMetadata.ContentType,
                                                 producesMetadata.AdditionalContentTypes);
                    break;
                case ProducesType.Problems:
                    routeHandlerBuilder.ProducesProblem((int)producesMetadata.StatusCode,
                                                        producesMetadata.ContentType);
                    break;
                case ProducesType.ValidationProblems:
                    routeHandlerBuilder.ProducesValidationProblem(contentType: producesMetadata.ContentType);
                    break;
            }
        }
    }

    private static void SetAcceptHeader(EndpointBase endpoint, RouteHandlerBuilder routeHandlerBuilder, ILogger logger) {
        foreach (var accept in endpoint.GetAccepts()) {
            try {
                routeHandlerBuilder.Accepts(requestType: accept.RequestType,
                                            isOptional: accept.IsOptional,
                                            contentType: accept.ContentType,
                                            additionalContentTypes: accept.AdditionalContentTypes);
            } catch (Exception ex) { logger.AcceptHeaderItemError(accept, ex); }
        }
    }

    private static bool TryCreateRouteHandlerBuilder(IEndpointRouteBuilder self, EndpointBase endpoint, [NotNullWhen(returnValue: true)] out RouteHandlerBuilder? routeHandlerBuilder) {
        routeHandlerBuilder = null;

        try {
            routeHandlerBuilder = endpoint.HttpMethod.ToUpperInvariant() switch {
                Root.HttpMethods.DELETE => self.MapDelete(endpoint.RoutePattern, endpoint.CreateDelegate()),
                Root.HttpMethods.GET => self.MapGet(endpoint.RoutePattern, endpoint.CreateDelegate()),
                Root.HttpMethods.PATCH => self.MapPatch(endpoint.RoutePattern, endpoint.CreateDelegate()),
                Root.HttpMethods.POST => self.MapPost(endpoint.RoutePattern, endpoint.CreateDelegate()),
                Root.HttpMethods.PUT => self.MapPut(endpoint.RoutePattern, endpoint.CreateDelegate()),
                _ => null
            };
        } catch { /* swallow */ }

        return routeHandlerBuilder is not null;
    }
}
