using Nameless.Mediator.Requests;
using Nameless.Validation;

namespace Nameless.Microservices.Application.Behaviors;

/// <summary>
/// Validation pipeline behavior.
/// </summary>
/// <typeparam name="TRequest">Type of the request.</typeparam>
/// <typeparam name="TResponse">Type of the response.</typeparam>
public class ValidatePipelineBehavior<TRequest, TResponse> : IRequestPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull {
    private readonly IValidationService _validationService;

    /// <summary>
    /// Initializes a new instance of <see cref="ValidatePipelineBehavior{TRequest, TResponse}"/>.
    /// </summary>
    /// <param name="validationService">The validation service.</param>
    public ValidatePipelineBehavior(IValidationService validationService) {
        _validationService = validationService;
    }

    /// <inheritdoc />
    public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
        var result = await _validationService.ValidateAsync(request, cancellationToken);

        if (!result.Succeeded) {
            throw new ValidationException(result);
        }

        return await next(cancellationToken);
    }
}
