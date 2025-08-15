using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Validation;
using HttpResults = Microsoft.AspNetCore.Http.TypedResults;

namespace Nameless.Web.Endpoints.Interception;

/// <summary>
///     An interceptor that validates endpoint requests using the
///     <see cref="IValidationService"/> service.
/// </summary>
public class ValidateEndpointInterceptor : EndpointInterceptorBase {
    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="ValidateEndpointInterceptor"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">
    ///     The HTTP context accessor.
    /// </param>
    public ValidateEndpointInterceptor(IHttpContextAccessor httpContextAccessor)
        : base(httpContextAccessor) { }

    public override async Task<IResult> InterceptAsync(HttpContext httpContext, object?[] arguments, CancellationToken cancellationToken) {
        var args = arguments.Where(ValidateAttribute.IsPresent);

        var validationService = httpContext.RequestServices.GetRequiredService<IValidationService>();
        var validations = new List<ValidationResult>();

        foreach (var arg in args) {
            if (arg is null) { continue; }

            validations.Add(await validationService.ValidateAsync(arg, cancellationToken));
        }

        var validationResult = validations.Aggregate();

        return validationResult.Succeeded
            ? InterceptorResult.Continue()
            : HttpResults.ValidationProblem(validationResult.ToDictionary());
    }
}
