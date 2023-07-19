using Autofac;
using FluentValidation;
using FluentValidation.Results;

namespace Nameless.FluentValidation {
    public sealed class ValidatorProvider : IValidatorProvider {
        #region Private Read-Only Fields

        private readonly ILifetimeScope _scope;

        #endregion

        #region Public Constructors

        public ValidatorProvider(ILifetimeScope scope) {
            _scope = Prevent.Against.Null(scope, nameof(scope));
        }

        #endregion

        #region IValidatorProvider Members

        public Task<ValidationResult> ValidateAsync<T>(T instance, bool throwOnError = false, CancellationToken cancellationToken = default) {
            Prevent.Against.Null(instance, nameof(instance));

            var validator = _scope.ResolveOptional<IValidator<T>>();

            if (validator == null) {
                return Task.FromResult(new ValidationResult());
            }

            return validator.ValidateAsync(
                instance,
                opts => { if (throwOnError) { opts.ThrowOnFailures(); } },
                cancellationToken
            );
        }

        #endregion
    }
}
