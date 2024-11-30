using MediatR;
using Nameless.Validation;

namespace Nameless.MediatR.Integration;

/// <summary>
/// Validation pipeline for <see cref="IRequest{TResponse}"/>.
/// </summary>
/// <typeparam name="TRequest">Type of the request.</typeparam>
/// <typeparam name="TResponse">Type of the response.</typeparam>
public sealed class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse> {
    private readonly IValidationService _validationService;

    /// <summary>
    /// Initializes a new instance of <see cref="ValidationPipelineBehavior{TRequest,TResponse}"/>.
    /// </summary>
    /// <param name="validationService">The validation service instance.</param>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="validationService"/> is <c>null</c>.
    /// </exception>
    public ValidationPipelineBehavior(IValidationService validationService) {
        _validationService = Prevent.Argument.Null(validationService);
    }

    /// <summary>
    /// Handles the request object validation, if a suitable validator is found.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="next">The next handler.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An awaitable task representing the validation process.</returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="request"/> or
    /// <paramref name="next"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ValidationException">
    /// If a suitable validator was found and if it fails the validation rules.
    /// </exception>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
        Prevent.Argument.Null(request);
        Prevent.Argument.Null(next);

        await _validationService.ValidateAsync(value: request,
                                               throwOnFailure: true,
                                               cancellationToken: cancellationToken)
                                .ConfigureAwait(continueOnCapturedContext: false);

        return await next().ConfigureAwait(continueOnCapturedContext: false);
    }
}