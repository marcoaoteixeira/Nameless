using FluentValidation.Results;

namespace Nameless.FluentValidation {
    public interface IValidatorService {
        #region Methods

        Task<ValidationResult> ValidateAsync<T>(T instance, bool throwOnError = false, CancellationToken cancellationToken = default);

        #endregion
    }
}
