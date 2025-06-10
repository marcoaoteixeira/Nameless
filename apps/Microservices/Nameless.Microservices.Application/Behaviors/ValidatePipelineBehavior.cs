using Microsoft.Extensions.Logging;
using Nameless.Mediator.Requests;
using Nameless.Validation;

namespace Nameless.Microservices.Application.Behaviors;

/// <summary>
/// Validation pipeline behavior.
/// </summary>
/// <typeparam name="TRequest">Type of the request.</typeparam>
/// <typeparam name="TResponse">Type of the response.</typeparam>
public class ValidatePipelineBehavior<TRequest, TResponse> : IRequestPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequestBase {
    private readonly IValidationService _validationService;
    private readonly ILogger<ValidatePipelineBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="ValidatePipelineBehavior{TRequest, TResponse}"/>.
    /// </summary>
    /// <param name="validationService">The validation service.</param>
    /// <param name="logger">The logger.</param>
    public ValidatePipelineBehavior(IValidationService validationService, ILogger<ValidatePipelineBehavior<TRequest, TResponse>> logger) {
        _validationService = Prevent.Argument.Null(validationService);
        _logger = Prevent.Argument.Null(logger);
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
