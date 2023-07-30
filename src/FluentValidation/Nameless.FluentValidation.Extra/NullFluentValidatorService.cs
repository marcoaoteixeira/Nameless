using FluentValidation.Results;

namespace Nameless.FluentValidation {
    public sealed class NullFluentValidatorService : IFluentValidatorService {
        #region Private Static Read-Only Fields

        private static readonly NullFluentValidatorService _instance = new();

        #endregion

        #region Public Static Read-Only Properties

        public static IFluentValidatorService Instance => _instance;

        #endregion

        #region Static Constructors

        static NullFluentValidatorService() { }

        #endregion

        #region Private Constructors

        private NullFluentValidatorService() { }

        #endregion

        #region IFluentValidatorService Members

        public Task<ValidationResult> ValidateAsync<T>(T instance, bool throwOnError = false, CancellationToken cancellationToken = default)
            => Task.FromResult(new ValidationResult());

        public Task<ValidationResult> ValidateAsync(object instance, bool throwOnError = false, CancellationToken cancellationToken = default)
            => Task.FromResult(new ValidationResult());

        #endregion
    }
}
