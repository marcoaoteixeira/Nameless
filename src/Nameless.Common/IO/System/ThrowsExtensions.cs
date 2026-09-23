using System.Diagnostics.CodeAnalysis;
using Nameless.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Nameless.IO.System;

[ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.Internal)]
internal static class ThrowsExtensions {
    extension(Throws self) {
        internal string DirectoryNotFound([NotNull] string? paramValue, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
            Throws.When.NullOrWhiteSpace(paramValue, paramName, message, exceptionCreator);

            if (SysDirectory.Exists(paramValue)) {
                return paramValue;
            }

            throw exceptionCreator?.Invoke() ?? new DirectoryNotFoundException(
                string.IsNullOrWhiteSpace(message)
                    ? $"Directory not found: {paramValue}"
                    : message
                );
        }

        internal string OutsideRootDirectory([NotNull] string? fullPath, string root, bool ignore = false, [CallerArgumentExpression(nameof(fullPath))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
            Throws.When.NullOrWhiteSpace(fullPath, paramName, message, exceptionCreator);
            Throws.When.NullOrWhiteSpace(root, nameof(root), message, exceptionCreator);

            if (ignore || PathHelper.Normalize(fullPath).StartsWith(root, StringComparison.Ordinal)) {
                return fullPath;
            }

            throw exceptionCreator?.Invoke() ?? new UnauthorizedAccessException(
                string.IsNullOrWhiteSpace(message)
                    ? $"Path must point to location inside root directory. Path: {fullPath}"
                    : message
            );
        }
    }
}