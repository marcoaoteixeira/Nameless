using System.Diagnostics.CodeAnalysis;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Validation;
using Nameless.Web.Infrastructure;

namespace Nameless.Web;

public static class ApplicationBuilderExtension {
    /// <summary>
    ///     Uses the JSON Web Token authentication services.
    /// </summary>
    /// <param name="self">The current <see cref="IApplicationBuilder" /> instance.</param>
    /// <returns>
    ///     The current <see cref="IApplicationBuilder" /> instance so other actions can be chained.
    /// </returns>
    public static IApplicationBuilder UseJwtAuth(this IApplicationBuilder self)
        => Prevent.Argument
                  .Null(self)
                  .UseMiddleware<JwtAuthorizationMiddleware>()
                  .UseAuthorization()
                  .UseAuthentication();

    /// <summary>
    ///     Uses the Swagger middleware with versioning capabilities.
    /// </summary>
    /// <param name="self">The current <see cref="IApplicationBuilder" /> instance.</param>
    /// <returns>
    ///     The current <see cref="IApplicationBuilder" /> instance so other actions can be chained.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     If <see cref="IApiVersionDescriptionProvider" /> is not present in the dependency container.
    /// </exception>
    public static IApplicationBuilder UseSwaggerWithVersioning(this IApplicationBuilder self) {
        Prevent.Argument.Null(self);

        var apiVersionDescriptionProvider = self.ApplicationServices
                                                .GetService<IApiVersionDescriptionProvider>();

        if (apiVersionDescriptionProvider is null) {
            throw new InvalidOperationException("Versioning not enabled.");
        }

        return self.UseSwagger()
                   .UseSwaggerUI(opts => {
                       foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions) {
                           opts.SwaggerEndpoint(url: $"/swagger/{description.GroupName}/swagger.json",
                                                name: description.GroupName.ToUpperInvariant());
                       }
                   });
    }

    /// <summary>
    ///     Configures API endpoints when implementing <see cref="MinimalEndpointBase" /> interface.
    ///     A call to <see cref="UseMinimalEndpoints" /> must be preceded by
    ///     a call to <see cref="EndpointRoutingApplicationBuilderExtensions.UseRouting" />.
    /// </summary>
    /// <param name="self">The current <see cref="IApplicationBuilder" /> instance.</param>
    /// <returns>
    ///     The current <see cref="IApplicationBuilder" /> instance so other actions can be chained.
    /// </returns>
    public static IApplicationBuilder UseMinimalEndpoints(this IApplicationBuilder self) {
        Prevent.Argument.Null(self);

        self.UseExceptionHandler(builder => builder.Run(ctx => TryHandleValidationException(ctx, out var result)
                                                            ? result.ExecuteAsync(ctx)
                                                            : Results.Problem().ExecuteAsync(ctx)));

        self.UseEndpoints(builder => {
            var endpoints = builder.ServiceProvider
                                   .GetServices<MinimalEndpointBase>();

            foreach (var endpoint in endpoints) {
                builder.Map(endpoint);
            }
        });

        return self;
    }

    private static TException? GetExceptionFromHttpContext<TException>(HttpContext httpContext)
        where TException : Exception {
        var feature = httpContext.Features.Get<IExceptionHandlerPathFeature>();
        return feature?.Error as TException;
    }

    private static bool TryHandleValidationException(HttpContext ctx, [NotNullWhen(true)] out IResult? result) {
        result = null;

        var ex = GetExceptionFromHttpContext<ValidationException>(ctx);
        if (ex is null) { return false; }

        result = Results.ValidationProblem(ex.Result.ToDictionary(),
                                           ex.Message);

        return true;
    }
}