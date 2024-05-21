using FluentValidation;
using MediatR;

namespace Nameless.MediatR.Integration {
    public sealed class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse> {
        #region Private Read-Only Fields

        private readonly IValidator<TRequest>? _validator;

        #endregion

        #region Public Constructors

        public ValidationPipelineBehavior(IValidator<TRequest>? validator = null) {
            _validator = validator;
        }

        #endregion

        #region IPipelineBehavior<TRequest, TResponse> Members

        /// <summary>
        /// Handles the request validation, if available.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="next">The next handler.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The final handler result.</returns>
        /// <exception cref="ValidationException">
        /// If <see cref="IValidator{T}"/> is available and the validation fails.
        /// </exception>
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
            if (_validator is not null) {
                await _validator.ValidateAsync(instance: request,
                                               options: opts => opts.ThrowOnFailures(),
                                               cancellation: cancellationToken)
                                .ConfigureAwait(continueOnCapturedContext: false);
            }

            return await next();
        }

        #endregion
    }
}
