using System.Diagnostics.CodeAnalysis;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Validation.Abstractions;
using Nameless.Web.Auth;
using Nameless.Web.Endpoints;

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
}