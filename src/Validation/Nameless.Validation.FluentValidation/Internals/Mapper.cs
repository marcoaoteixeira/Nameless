﻿using FluentValidationResult = FluentValidation.Results.ValidationResult;

namespace Nameless.Validation.FluentValidation.Internals;

internal static class Mapper {
    internal static ValidationResult Map(FluentValidationResult result) {
        if (result.IsValid) { return ValidationResult.Empty; }

        var entries = result.Errors
                            .Select(error => new ValidationError(error.PropertyName ?? error.ErrorCode,
                                                                 error.ErrorMessage))
                            .ToArray();

        return new ValidationResult(entries);
    }
}