using Nameless.Mediator.Requests;
using Nameless.Validation;

namespace Nameless.Microservices.App.PipelineBehaviors;

/// <summary>
/// Validation pipeline behavior.
/// </summary>
/// <typeparam name="TRequest">Type of the request.</typeparam>
/// <typeparam name="TResponse">Type of the response.</typeparam>
public class ValidateRequestPipelineBehavior<TRequest, TResponse> : IRequestPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequestBase {
    private readonly IValidationService _validationService;
    private readonly ILogger<ValidateRequestPipelineBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="ValidateRequestPipelineBehavior{TRequest,TResponse}"/>.
    /// </summary>
    /// <param name="validationService">The validation service.</param>
    /// <param name="logger">The logger.</param>
    public ValidateRequestPipelineBehavior(IValidationService validationService, ILogger<ValidateRequestPipelineBehavior<TRequest, TResponse>> logger) {
        _validationService = Guard.Against.Null(validationService);
        _logger = Guard.Against.Null(logger);
    }

    /// <inheritdoc />
    public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
        var result = await _validationService.ValidateAsync(request, cancellationToken);

        if (result.Succeeded) {
            return await next(cancellationToken);
        }

        _logger.ValidateRequestFailure(result);

        throw new ValidationException(result);
    }
}
