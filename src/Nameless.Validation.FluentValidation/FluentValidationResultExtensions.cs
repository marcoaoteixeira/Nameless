using FluentValidationResult = FluentValidation.Results.ValidationResult;

namespace Nameless.Validation.FluentValidation;

public static class FluentValidationResultExtensions {
    public static ValidationResult ToValidationResult(this IEnumerable<FluentValidationResult> self) {
        var validationErrorCollection = self.Where(item => !item.IsValid)
                                            .SelectMany(item => item.Errors)
                                            .Select(error => new ValidationError(
                                                 error.ErrorMessage,
                                                 error.ErrorCode,
                                                 error.PropertyName))
                                            .ToList();

        return validationErrorCollection.Count != 0
            ? ValidationResult.Failure(validationErrorCollection)
            : ValidationResult.Success();
    }
}