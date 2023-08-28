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

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
            if (_validator is not null) {
                await _validator
                    .ValidateAsync(request, opts => opts.ThrowOnFailures(), cancellationToken)
                    .ConfigureAwait(false);
            }

            return await next();
        }

        #endregion
    }
}
