using MediatR;
using Nameless.Validation.Abstractions;

namespace Nameless.MediatR.Integration {
    public sealed class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse> {
        #region Private Read-Only Fields

        private readonly IValidationService _validationService;

        #endregion

        #region Public Constructors

        public ValidationPipelineBehavior(IValidationService validationService) {
            _validationService = validationService;
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
        /// If validation fails.
        /// </exception>
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
            if (request.GetType().IsValueType) {
                await _validationService.ValidateAsync(value: (object)request,
                                                       throwOnFailure: true,
                                                       cancellationToken: cancellationToken)
                                        .ConfigureAwait(continueOnCapturedContext: false);
            }

            return await next();
        }

        #endregion
    }
}
