using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Nameless.Web.Filters.Validation;

public class ValidateRequestAsyncActionFilter : ValidationFilterBase, IAsyncActionFilter {
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
        var result = await ValidateRequestObjectsAsync(
            context.HttpContext.RequestServices,
            context.ActionArguments.Values,
            context.HttpContext.RequestAborted
        );

        await result.Match(
            onSuccess: _ => next(),
            onFailure: errors => {
                context.Result = new BadRequestObjectResult(errors.ToModelStateDictionary());

                return Task.CompletedTask;
            }
        );
    }
}