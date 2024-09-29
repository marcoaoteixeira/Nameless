using System.Diagnostics.CodeAnalysis;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Validation.Abstractions;
using Nameless.Web.Api;
using Nameless.Web.Auth;
using Nameless.Web.Filters;
using Nameless.Web.Internals;

namespace Nameless.Web;
public static class ApplicationBuilderExtension {
    public static IApplicationBuilder UseValidationExceptionTreatment(this IApplicationBuilder self)
        => self.UseExceptionHandler(builder => builder.Run(ctx => TryHandleValidationException(ctx, out var result)
                                                           ? result.ExecuteAsync(ctx)
                                                           : Results.Problem().ExecuteAsync(ctx)));

    public static IApplicationBuilder UseJwtAuth(this IApplicationBuilder self)
        => self.UseMiddleware<JwtAuthorizationMiddleware>()
               .UseAuthorization()
               .UseAuthentication();

    /// <summary>
    /// Configures API endpoints when implementing <see cref="IEndpoint"/> interface.
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
                                   .GetServices<IEndpoint>();

            foreach (var endpoint in endpoints) {
                if (string.IsNullOrWhiteSpace(endpoint.RoutePattern)) {
                    logger.EndpointMissingRoutePattern(endpoint);

                    continue;
                }

                // let's try map our endpoint.
                var routeHandlerBuilder = endpoint.HttpMethod switch {
                    Root.HttpMethods.DELETE => builder.MapDelete(endpoint.RoutePattern, endpoint.GetHandler()),
                    Root.HttpMethods.GET => builder.MapGet(endpoint.RoutePattern, endpoint.GetHandler()),
                    Root.HttpMethods.PATCH => builder.MapPatch(endpoint.RoutePattern, endpoint.GetHandler()),
                    Root.HttpMethods.POST => builder.MapPost(endpoint.RoutePattern, endpoint.GetHandler()),
                    Root.HttpMethods.PUT => builder.MapPut(endpoint.RoutePattern, endpoint.GetHandler()),
                    _ => null
                };

                if (routeHandlerBuilder is null) {
                    logger.EndpointNotMapped(endpoint);

                    continue;
                }

                // add metadata for Open API
                routeHandlerBuilder.WithOpenApi();

                var attributes = GetEndpointHandlerAttributes(endpoint);

                foreach (var attribute in attributes) {
                    switch (attribute) {
                        case EndpointNameAttribute attr:
                            routeHandlerBuilder.WithName(attr.EndpointName);
                            break;
                        case EndpointSummaryAttribute attr:
                            routeHandlerBuilder.WithSummary(attr.Summary);
                            break;
                        case EndpointDescriptionAttribute attr:
                            routeHandlerBuilder.WithDescription(attr.Description);
                            break;
                        case EndpointGroupNameAttribute attr:
                            routeHandlerBuilder.WithGroupName(attr.EndpointGroupName)
                                               .WithApiVersionSet(builder.NewApiVersionSet(attr.EndpointGroupName)
                                                                         .Build());
                            break;
                        case MapToApiVersionAttribute attr:
                            foreach (var apiVersion in attr.Versions) {
                                routeHandlerBuilder.MapToApiVersion(apiVersion);
                            }
                            break;
                        case ApiVersionAttribute attr:
                            foreach (var apiVersion in attr.Versions) {
                                routeHandlerBuilder.HasApiVersion(apiVersion);
                                if (attr.Deprecated) {
                                    routeHandlerBuilder.HasDeprecatedApiVersion(apiVersion);
                                }
                            }
                            break;
                        case UseEndpointValidationAttribute:
                            routeHandlerBuilder.AddEndpointFilter(new ValidateEndpointFilter());
                            break;
                    }
                }
            }
        });

        return self;
    }

    public static IApplicationBuilder UseSwaggerWithVersioning(this IApplicationBuilder self) {
        var apiVersionDescriptionProvider = self.ApplicationServices
                                                .GetService<IApiVersionDescriptionProvider>();

        if (apiVersionDescriptionProvider is null) {
            throw new InvalidOperationException($"Dependency injection missing service {nameof(IApiVersionDescriptionProvider)}");
        }

        return self.UseSwagger()
                   .UseSwaggerUI(opts => {
                       foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions) {
                           opts.SwaggerEndpoint(url: $"/swagger/{description.GroupName}/swagger.json",
                                                name: description.GroupName.ToUpperInvariant());
                       }
                   });
    }

    private static TException? GetExceptionFromHttpContext<TException>(HttpContext httpContext)
        where TException : Exception {
        var feature = httpContext.Features.Get<IExceptionHandlerPathFeature>();
        return feature?.Error as TException;
    }

    private static bool TryHandleValidationException(HttpContext ctx, [NotNullWhen(returnValue: true)] out IResult? result) {
        result = null;

        var ex = GetExceptionFromHttpContext<ValidationException>(ctx);
        if (ex is null) { return false; }

        result = Results.ValidationProblem(errors: ex.Result.ToDictionary(),
                                           detail: ex.Message);

        return true;
    }
    
    private static IEnumerable<Attribute> GetEndpointHandlerAttributes(IEndpoint endpoint) {
        var handler = endpoint.GetType()
                              .GetMethod(nameof(IEndpoint.GetHandler));

        if (handler is null) { return []; }

        return handler.GetCustomAttributes(inherit: false)
                      .OfType<Attribute>();
    }
}