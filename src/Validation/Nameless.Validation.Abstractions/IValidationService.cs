namespace Nameless.Validation.Abstractions {
    public interface IValidationService {
        #region Methods

        Task<ValidationResult> ValidateAsync<TValue>(TValue value, bool throwOnFailure, CancellationToken cancellationToken) where TValue : class;

        #endregion
    }
}