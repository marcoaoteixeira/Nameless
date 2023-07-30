using Autofac;
using FluentValidation;
using FluentValidation.Results;

namespace Nameless.FluentValidation {
    public sealed class FluentValidatorService : IFluentValidatorService {
        #region Private Read-Only Fields

        private readonly ILifetimeScope _scope;

        #endregion

        #region Public Constructors

        public FluentValidatorService(ILifetimeScope scope) {
            _scope = Prevent.Against.Null(scope, nameof(scope));
        }

        #endregion

        #region IFluentValidatorService Members

        public Task<ValidationResult> ValidateAsync<T>(T instance, bool throwOnError = false, CancellationToken cancellationToken = default) {
            Prevent.Against.Null(instance, nameof(instance));

            var validator = _scope.Resolve<IValidator<T>>();

            return validator.ValidateAsync(
                instance,
                opts => { if (throwOnError) { opts.ThrowOnFailures(); } },
                cancellationToken
            );
        }

        public async Task<ValidationResult> ValidateAsync(object instance, bool throwOnError = false, CancellationToken cancellationToken = default) {
            Prevent.Against.Null(instance, nameof(instance));

            var instanceType = instance.GetType();
            var validatorType = typeof(IValidator<>).MakeGenericType(instanceType);
            var validator = _scope.Resolve(validatorType) as IValidator
                ?? throw new InvalidOperationException($"Could not found validator for {instanceType.FullName}");
            var validationContextType = typeof(ValidationContext<>).MakeGenericType(instanceType);
            var validationContext = Activator.CreateInstance(validationContextType, instance) as IValidationContext;

            var validationResult = await validator.ValidateAsync(validationContext, cancellationToken);
            if (validationResult.Failure() && throwOnError) {
                throw new ValidationException(validationResult.Errors);
            }

            return validationResult;
        }

        #endregion
    }
}
