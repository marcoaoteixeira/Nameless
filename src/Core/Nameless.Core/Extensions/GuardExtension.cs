﻿using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Nameless {
    public static class GuardExtension {
        #region Private Constants

        private const string ARG_NULL_EX_MESSAGE = "Argument cannot be null.";
        private const string ARG_EMPTY_EX_MESSAGE = "Argument cannot be empty.";
        private const string ARG_EMPTY_WHITESPACES_EX_MESSAGE = "Argument cannot be white spaces.";
        private const string ARG_NO_MATCH_PATTER_EX_MESSAGE = "Argument does not match pattern.";

        #endregion

        #region Public Static Methods

        [DebuggerStepThrough]
        public static T Null<T>(this Guard _, [NotNull] T? input, string name, string? message = null)
            => input ?? throw new ArgumentNullException(name, message ?? ARG_NULL_EX_MESSAGE);

        [DebuggerStepThrough]
        public static string NullOrEmpty(this Guard _, [NotNull] string? input, string name, string? message = null) {
            if (input is null) {
                throw new ArgumentNullException(name, message ?? ARG_NULL_EX_MESSAGE);
            }

            if (input.Length == 0) {
                throw new ArgumentException(message ?? ARG_EMPTY_EX_MESSAGE, name);
            }

            return input;
        }

        [DebuggerStepThrough]
        public static string NullOrWhiteSpace(this Guard _, [NotNull] string? input, string name, string? message = null) {
            if (input is null) {
                throw new ArgumentNullException(name, message ?? ARG_NULL_EX_MESSAGE);
            }

            if (input.Trim().Length == 0) {
                throw new ArgumentException(message ?? ARG_EMPTY_WHITESPACES_EX_MESSAGE, name);
            }

            return input;
        }

        [DebuggerStepThrough]
        public static T NullOrEmpty<T>(this Guard _, [NotNull] T? input, string name, string? message = null)
            where T : class, IEnumerable {
            if (input is null) {
                throw new ArgumentNullException(name, message ?? ARG_NULL_EX_MESSAGE);
            }

            // Costs O(1)
            if (input is ICollection collection && collection.Count == 0) {
                throw new ArgumentException(message ?? ARG_EMPTY_EX_MESSAGE, name);
            }

            // Costs O(N)
            var enumerator = input.GetEnumerator();
            var canMoveNext = enumerator.MoveNext();
            if (enumerator is IDisposable disposable) {
                disposable.Dispose();
            }
            if (!canMoveNext) {
                throw new ArgumentException(message ?? ARG_EMPTY_EX_MESSAGE, name);
            }
            return input;
        }

        [DebuggerStepThrough]
        public static string NoMatchingPattern(this Guard _, string input, string name, string pattern, string? message = null) {
            var match = Regex.Match(input, pattern);
            if (!match.Success || match.Value != input) {
                throw new ArgumentException(message ?? ARG_NO_MATCH_PATTER_EX_MESSAGE, name);
            }
            return input;
        }

        #endregion
    }
}