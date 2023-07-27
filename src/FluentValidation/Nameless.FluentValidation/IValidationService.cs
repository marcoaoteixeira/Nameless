using FluentValidation.Results;

namespace Nameless.FluentValidation {
    public interface IValidationService {
        #region Methods

        Task<ValidationResult> ValidateAsync<T>(T instance, bool throwOnError = false, CancellationToken cancellationToken = default);
        Task<ValidationResult> ValidateAsync(object instance, bool throwOnError = false, CancellationToken cancellationToken = default);

        #endregion
    }
}