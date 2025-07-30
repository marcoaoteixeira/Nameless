using Microsoft.AspNetCore.Http;
using Nameless.Validation;
using Nameless.Web.Endpoints.Definitions;

using HttpResults = Microsoft.AspNetCore.Http.TypedResults;

namespace Nameless.Web.Endpoints.Infrastructure;

/// <summary>
///     An interceptor that validates endpoint requests using the
///     <see cref="IValidationService"/> service.
/// </summary>
public class ValidateEndpointInterceptor : EndpointInterceptor {
    private readonly IValidationService _validationService;

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="ValidateEndpointInterceptor"/> class.
    /// </summary>
    /// <param name="validationService">
    ///     The validation service used to validate the endpoint
    ///     requests objects.
    /// </param>
    /// <param name="httpContextAccessor">
    ///     The HTTP context accessor.
    /// </param>
    public ValidateEndpointInterceptor(
        IValidationService validationService,
        IHttpContextAccessor httpContextAccessor
    ) : base(httpContextAccessor) {
        _validationService = Prevent.Argument.Null(validationService);
    }

    public override bool CanIntercept(IEndpointDescriptor descriptor) {
        return typeof(IEndpoint).IsAssignableFrom(descriptor.EndpointType);
    }

    public override async Task<IResult> InterceptAsync(object[] arguments, CancellationToken cancellationToken) {
        var args = arguments.Where(ValidateAttribute.IsPresent);

        var validations = new List<ValidationResult>();

        foreach (var arg in args) {
            validations.Add(await _validationService.ValidateAsync(arg, cancellationToken));
        }

        var validationResult = validations.Aggregate();

        return validationResult.Succeeded
            ? HttpResults.Empty
            : HttpResults.ValidationProblem(validationResult.ToDictionary());
    }
}