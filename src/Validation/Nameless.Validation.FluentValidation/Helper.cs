﻿using Nameless.Validation.Abstractions;
using FluentValidationResult = FluentValidation.Results.ValidationResult;

namespace Nameless.Validation.FluentValidation;

internal static class Helper {
    #region Internal Static Methods

    internal static ValidationResult Map(FluentValidationResult result) {
        if (result.IsValid) { return ValidationResult.Empty; }

        var entries = result.Errors
                            .Select(error => new ValidationEntry(error.PropertyName ?? error.ErrorCode,
                                                                 error.ErrorMessage))
                            .ToArray();

        return new ValidationResult(entries);
    }

    #endregion
}