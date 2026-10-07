using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Nameless.Diagnostics.CodeAnalysis;

namespace Nameless.IO;

[ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.Internal)]
internal static class ThrowsExtensions {
    private static readonly SearchValues<char> InvalidPathChars = SearchValues.Create(
        [.. SysPath.GetInvalidFileNameChars()
                   .Where(@char => @char != SysPath.AltDirectorySeparatorChar &&
                                   @char != SysPath.DirectorySeparatorChar)]
    );

    private static readonly StringComparison PathComparison = OperatingSystem.IsWindows()
        ? StringComparison.OrdinalIgnoreCase
        : StringComparison.Ordinal;

    extension(Throws self) {
        [DebuggerStepThrough]
        internal string PathHasInvalidChars(string path, [CallerArgumentExpression(nameof(path))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
            if (!path.AsSpan().ContainsAny(InvalidPathChars)) {
                return path;
            }

            throw exceptionCreator?.Invoke() ?? new ArgumentException(
                string.IsNullOrWhiteSpace(message)
                    ? $"The path has one or more invalid chars: {string.Join(' ', InvalidPathChars)}"
                    : message,
                paramName
            );
        }

        [DebuggerStepThrough]
        internal string PathIsRooted(string path, [CallerArgumentExpression(nameof(path))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
            if (!SysPath.IsPathRooted(path)) {
                return path;
            }

            throw exceptionCreator?.Invoke() ?? new ArgumentException(
                string.IsNullOrWhiteSpace(message)
                    ? "The path cannot be absolute"
                    : message,
                paramName
            );
        }

        [DebuggerStepThrough]
        internal string PathIsNotRooted(string path, [CallerArgumentExpression(nameof(path))] string? paramName = null, string? message = null, Func<Exception>? exceptionCreator = null) {
            if (SysPath.IsPathRooted(path)) {
                return path;
            }

            throw exceptionCreator?.Invoke() ?? new ArgumentException(
                string.IsNullOrWhiteSpace(message)
                    ? "The path must be absolute"
                    : message,
                paramName
            );
        }

        [DebuggerStepThrough]
        internal string PathNavigatesAboveRoot(string relativePath, string? message = null, Func<Exception>? exceptionCreator = null) {
            if (!PathHelper.NavigatesAboveRoot(relativePath)) {
                return relativePath;
            }

            throw exceptionCreator?.Invoke() ?? new RelativePathException(
                string.IsNullOrWhiteSpace(message)
                    ? $"The path '{relativePath}' escapes root."
                    : message,
                relativePath
            );
        }

        [DebuggerStepThrough]
        internal string PathUnderneathRoot(string root, string relativePath, string paramValue, string? message = null, Func<Exception>? exceptionCreator = null) {
            var isUnderneath = string.Equals(paramValue, root, PathComparison) ||
                               paramValue.StartsWith(root, PathComparison);

            if (isUnderneath) {
                return paramValue;
            }

            throw exceptionCreator?.Invoke() ?? new RelativePathException(
                string.IsNullOrWhiteSpace(message)
                    ? $"The path '{relativePath}' escapes root."
                    : message,
                relativePath
            );
        }
    }
}