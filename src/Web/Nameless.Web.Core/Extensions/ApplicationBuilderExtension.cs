using System.Diagnostics.CodeAnalysis;
using Asp.Versioning.ApiExplorer;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.Validation.Abstractions;
using Nameless.Web.Api;
using Nameless.Web.Auth;
using Nameless.Web.Filters;

namespace Nameless.Web;
public static class ApplicationBuilderExtension {
    public static IApplicationBuilder UseAutofacDestroyRoutine(this IApplicationBuilder self, IHostApplicationLifetime lifetime) {
        Prevent.Argument.Null(self);

        // Tear down the composition root and free all resources
        // when the application stops.
        var container = self.ApplicationServices.GetAutofacRoot();
        lifetime.ApplicationStopped.Register(container.Dispose);

        return self;
    }

    public static IApplicationBuilder UseValidationExceptionTreatment(this IApplicationBuilder self)
        => self.UseExceptionHandler(builder => builder.Run(ctx => TryHandleValidationException(ctx, out var result)
                                                           ? result.ExecuteAsync(ctx)
                                                           : Results.Problem().ExecuteAsync(ctx)));

    public static IApplicationBuilder UseJwtAuth(this IApplicationBuilder self)
        => self.UseMiddleware<JwtAuthorizationMiddleware>()
               .UseAuthorization()
               .UseAuthentication();

    /// <summary>
    /// Configures endpoints that are defined by <see cref="IEndpoint"/> interface.
    /// A call to <see cref="UseApiEndpoints"/> must be preceded by a call to <see cref="EndpointRoutingApplicationBuilderExtensions.UseRouting"/>.
    /// </summary>
    /// <param name="self">The current <see cref="IApplicationBuilder"/> instance.</param>
    /// <param name="useRequestValidation">
    /// Whether it should use <see cref="IValidationService"/> to validate request objects.
    /// This should be combined with the use of <see cref="ValidateAttribute"/> on the request
    /// parameters that might need be validated.
    /// </param>
    /// <returns>
    /// The current <see cref="IApplicationBuilder"/> instance so other actions can be chained.
    /// </returns>
    public static IApplicationBuilder UseApiEndpoints(this IApplicationBuilder self, bool useRequestValidation = true) {
        Prevent.Argument.Null(self);

        self.UseEndpoints(builder => {
            var endpoints = builder.ServiceProvider.GetServices<IEndpoint>();
            foreach (var endpoint in endpoints) {
                var convention = endpoint.Map(builder)
                                         .WithOpenApi()
                                         .WithName(endpoint.Name)
                                         .WithSummary(endpoint.Summary)
                                         .WithDescription(endpoint.Description)
                                         .WithApiVersionSet(builder.NewApiVersionSet(endpoint.Group)
                                                                   .Build())
                                         .HasApiVersion(endpoint.Version);

                if (useRequestValidation) {
                    convention.AddEndpointFilter(new ValidateEndpointFilter());
                }
            }
        });

        return self;
    }

    public static IApplicationBuilder UseSwagger(this IApplicationBuilder self, IApiVersionDescriptionProvider versioning)
        => self.UseSwagger()
               .UseSwaggerUI(swaggerUiOptions => {
                   foreach (var description in versioning.ApiVersionDescriptions) {
                       swaggerUiOptions.SwaggerEndpoint(
                           url: $"/swagger/{description.GroupName}/swagger.json",
                           name: description.GroupName.ToUpperInvariant()
                       );
                   }
               });

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
}