using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Validation.Abstractions;

namespace Nameless.Web.Filters {
    public sealed class ValidateEndpointFilter : IEndpointFilter {
        #region IEndpointFilter Members

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next) {
            var validationService = context.HttpContext.RequestServices.GetService<IValidationService>();

            if (validationService is null) { return await next(context); }

            var args = context.Arguments
                              .Where(ValidateAttribute.Present);

            var cancellationToken = context.HttpContext.RequestAborted;

            foreach (var arg in args) {
                if (arg is null) { continue; }

                var result = await validationService.ValidateAsync(arg, throwOnFailure: false, cancellationToken);

                if (!result.Succeeded) {
                    return Results.ValidationProblem(result.ToDictionary());
                }
            }

            return await next(context);
        }

        #endregion
    }
}
