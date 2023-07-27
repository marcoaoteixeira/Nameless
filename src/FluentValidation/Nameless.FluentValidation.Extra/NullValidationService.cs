using FluentValidation.Results;

namespace Nameless.FluentValidation {
    public sealed class NullValidationService : IValidationService {
        #region Private Static Read-Only Fields

        private static readonly NullValidationService _instance = new();

        #endregion

        #region Public Static Read-Only Properties

        public static IValidationService Instance => _instance;

        #endregion

        #region Static Constructors

        static NullValidationService() { }

        #endregion

        #region Private Constructors

        private NullValidationService() { }

        #endregion

        #region IValidationService Members

        public Task<ValidationResult> ValidateAsync<T>(T instance, bool throwOnError = false, CancellationToken cancellationToken = default)
            => Task.FromResult(new ValidationResult());

        public Task<ValidationResult> ValidateAsync(object instance, bool throwOnError = false, CancellationToken cancellationToken = default)
            => Task.FromResult(new ValidationResult());

        #endregion
    }
}
