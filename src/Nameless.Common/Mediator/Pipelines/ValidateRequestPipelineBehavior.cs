using Microsoft.Extensions.Logging;
using Nameless.Mediator.Requests;
using Nameless.Validation;

namespace Nameless.Mediator.Pipelines;

/// <summary>
/// Validation pipeline behavior.
/// </summary>
/// <typeparam name="TRequest">Type of the request.</typeparam>
/// <typeparam name="TResponse">Type of the response.</typeparam>
public class ValidateRequestPipelineBehavior<TRequest, TResponse> : IRequestPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull {
    private readonly IValidator _validator;
    private readonly ILogger<ValidateRequestPipelineBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="ValidateRequestPipelineBehavior{TRequest,TResponse}"/>.
    /// </summary>
    /// <param name="validator">The validation service.</param>
    /// <param name="logger">The logger.</param>
    public ValidateRequestPipelineBehavior(IValidator validator, ILogger<ValidateRequestPipelineBehavior<TRequest, TResponse>> logger) {
        _validator = validator;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
        var result = await _validator.ValidateAsync(request, cancellationToken);

        if (result.Success) {
            return await next(cancellationToken);
        }

        Log.ValidationFailure(_logger, result.Errors);

        throw new ValidationException(result);
    }
}
