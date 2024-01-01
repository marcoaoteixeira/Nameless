using FluentValidation.Results;

namespace Nameless.FluentValidation {
    public interface IValidatorManager {
        #region Methods

        Task<ValidationResult> ValidateAsync<T>(T instance, CancellationToken cancellationToken = default);

        #endregion
    }
}
