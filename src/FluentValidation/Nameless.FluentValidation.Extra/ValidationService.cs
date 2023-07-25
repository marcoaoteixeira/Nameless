using Autofac;
using FluentValidation;
using FluentValidation.Results;

namespace Nameless.FluentValidation {
    public sealed class ValidationService : IValidationService {
        #region Private Read-Only Fields

        private readonly ILifetimeScope _scope;

        #endregion

        #region Public Constructors

        public ValidationService(ILifetimeScope scope) {
            _scope = Prevent.Against.Null(scope, nameof(scope));
        }

        #endregion

        #region IValidatorProvider Members

        public Task<ValidationResult> ValidateAsync<T>(T instance, bool throwOnError = false, CancellationToken cancellationToken = default) {
            Prevent.Against.Null(instance, nameof(instance));

            var validator = _scope.Resolve<IValidator<T>>();

            return validator.ValidateAsync(
                instance,
                opts => { if (throwOnError) { opts.ThrowOnFailures(); } },
                cancellationToken
            );
        }

        #endregion
    }
}
