using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Nameless.Diagnostics.CodeAnalysis;

namespace Nameless.IO.System;

[ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.Internal)]
internal static class ThrowsExtensions {
    extension(Throws self) {
        [DebuggerStepThrough]
        internal string HasInvalidPathChars(string paramValue, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
            if (!PathUtils.HasInvalidPathChars(paramValue)) {
                return paramValue;
            }

            throw exceptionCreator?.Invoke() ?? new ArgumentException(
                string.IsNullOrWhiteSpace(message)
                    ? "The path contains invalid chars"
                    : message,
                paramName
            );
        }

        [DebuggerStepThrough]
        internal string PathIsRooted(string paramValue, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
            if (!SysPath.IsPathRooted(paramValue)) {
                return paramValue;
            }

            throw exceptionCreator?.Invoke() ?? new ArgumentException(
                string.IsNullOrWhiteSpace(message)
                    ? "The path cannot be absolute"
                    : message,
                paramName
            );
        }

        [DebuggerStepThrough]
        internal string PathIsNotRooted(string paramValue, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
            if (SysPath.IsPathRooted(paramValue)) {
                return paramValue;
            }

            throw exceptionCreator?.Invoke() ?? new ArgumentException(
                string.IsNullOrWhiteSpace(message)
                    ? "The path must be absolute"
                    : message,
                paramName
            );
        }
        
        [DebuggerStepThrough]
        internal string PathNavigatesAboveRoot(string paramValue, string? message = null, Func<Exception>? exceptionCreator = null) {
            if (!PathUtils.PathNavigatesAboveRoot(paramValue)) {
                return paramValue;
            }

            throw exceptionCreator?.Invoke() ?? new UnauthorizedAccessException(
                string.IsNullOrWhiteSpace(message)
                    ? "Path must point to location inside root directory."
                    : message
            );
        }

        [DebuggerStepThrough]
        internal string PathNotUnderneathRoot(string paramValue, string root, [CallerArgumentExpression(nameof(paramValue))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
            Throws.When.NullOrWhiteSpace(paramValue, paramName, message, exceptionCreator);
            Throws.When.NullOrWhiteSpace(root);

            if (paramValue.StartsWith(root, StringComparison.OrdinalIgnoreCase)) {
                return paramValue;
            }

            throw exceptionCreator?.Invoke() ?? new UnauthorizedAccessException(
                string.IsNullOrWhiteSpace(message)
                    ? "Path must be underneath root directory."
                    : message
            );
        }
    }
}