using Autofac;
using FluentValidation;
using FluentValidation.Results;

namespace Nameless.FluentValidation {
    public sealed class ValidatorManager : IValidatorManager {
        #region Private Read-Only Fields

        private readonly ILifetimeScope _scope;

        #endregion

        #region Public Constructors

        public ValidatorManager(ILifetimeScope scope) {
            _scope = Guard.Against.Null(scope, nameof(scope));
        }

        #endregion

        #region IValidatorManager Members

        public Task<ValidationResult> ValidateAsync<T>(T instance, bool throwOnError = false, CancellationToken cancellationToken = default) {
            Guard.Against.Null(instance, nameof(instance));

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
