using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Validation;

using HttpResults = Microsoft.AspNetCore.Http.Results;

namespace Nameless.Web.Filters;

/// <summary>
///     Endpoint filter that provides validation capabilities.
/// </summary>
public sealed class ValidateRequestEndpointFilter : IEndpointFilter {
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next) {
        var validationService = context.HttpContext
                                       .RequestServices
                                       .GetService<IValidationService>();

        var logger = context.HttpContext
                            .RequestServices
                            .GetLogger<ValidateRequestEndpointFilter>();

        if (validationService is null) {
            logger.ValidationServiceUnavailable();

            return await next(context);
        }

        var args = context.Arguments
                          .Cast<object>()
                          .Where(ValidateAttribute.IsPresent);

        var cancellationToken = context.HttpContext
                                       .RequestAborted;

        foreach (var arg in args) {
            if (arg is null) { continue; }

            var result = await validationService.ValidateAsync(arg, cancellationToken);

            if (!result.Succeeded) {
                return HttpResults.ValidationProblem(result.ToDictionary());
            }
        }

        return await next(context);
    }
}