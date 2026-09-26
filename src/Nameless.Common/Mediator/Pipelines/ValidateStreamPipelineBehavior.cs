using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Nameless.Mediator.Streams;
using Nameless.Validation;

namespace Nameless.Mediator.Pipelines;

/// <summary>
/// Validation pipeline behavior.
/// </summary>
/// <typeparam name="TRequest">Type of the request.</typeparam>
/// <typeparam name="TResponse">Type of the response.</typeparam>
public class ValidateStreamPipelineBehavior<TRequest, TResponse> : IStreamPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull {
    private readonly IValidator _validator;
    private readonly ILogger<ValidateStreamPipelineBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="ValidateStreamPipelineBehavior{TRequest,TResponse}"/>.
    /// </summary>
    /// <param name="validator">The validation service.</param>
    /// <param name="logger">The logger.</param>
    public ValidateStreamPipelineBehavior(IValidator validator, ILogger<ValidateStreamPipelineBehavior<TRequest, TResponse>> logger) {
        _validator = validator;
        _logger = logger;
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<TResponse> HandleAsync(TRequest request, StreamHandlerDelegate<TResponse> next, [EnumeratorCancellation] CancellationToken cancellationToken) {
        var result = await _validator.ValidateAsync(request, cancellationToken);

        if (result.Success) {
            await foreach (var item in next().WithCancellation(cancellationToken)) {
                yield return item;
            }
        }

        Log.ValidationFailure(_logger, result.Errors, GetType().Tag);

        throw new ValidationException(result);
    }
}
