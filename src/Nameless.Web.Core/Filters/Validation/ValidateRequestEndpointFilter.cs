using Microsoft.AspNetCore.Http;
using HttpResults = Microsoft.AspNetCore.Http.Results;

namespace Nameless.Web.Filters.Validation;

/// <summary>
///     Endpoint filter that provides validation capabilities.
/// </summary>
public class ValidateRequestEndpointFilter : ValidationFilterBase, IEndpointFilter {
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next) {
        var result = await ValidateRequestObjectsAsync(
            context.HttpContext.RequestServices,
            context.Arguments,
            context.HttpContext.RequestAborted
        );

        return await result.Match(
            onSuccess: _ => next(context),
            onFailure: errors => ValueTask.FromResult<object?>(
                HttpResults.ValidationProblem(errors.ToDictionary())
            )
        );
    }
}