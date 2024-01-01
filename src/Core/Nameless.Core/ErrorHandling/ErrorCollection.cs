﻿using System.Collections;

namespace Nameless.ErrorHandling {
    public sealed class ErrorCollection : IEnumerable<Error> {
        #region Public Static Read-Only Properties

        public static ErrorCollection Empty => new();

        #endregion

        #region Private Read-Only Properties

        private Dictionary<string, ISet<string>> Errors { get; } = new(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region Public Constructors

        public ErrorCollection() { }

        public ErrorCollection(IDictionary<string, string[]> errors) {
            foreach (var error in errors) {
                Push(error.Key, error.Value);
            }
        }

        #endregion

        #region Public Methods

        public void Push(string code, params string[] problems) {
            Guard.Against.Null(code, nameof(code));
            Guard.Against.Null(problems, nameof(problems));

            var error = AssertError(code);

            PushProblems(error, problems);
        }

        #endregion

        #region Private Static Methods

        private static void PushProblems(ISet<string> entry, string[] problems) {
            foreach (var problem in problems) {
                if (string.IsNullOrWhiteSpace(problem)) {
                    continue;
                }
                entry.Add(problem);
            }
        }

        #endregion

        #region Private Methods

        private ISet<string> AssertError(string code) {
            if (!Errors.TryGetValue(code, out var value)) {
                value = new HashSet<string>();
                Errors.Add(code, value);
            }
            return value;
        }

        private IEnumerable<Error> GetEnumerable() {
            foreach (var item in Errors) {
                yield return new(item.Key, [.. item.Value]);
            }
        }

        #endregion

        #region IEnumerable<Error> Members

        public IEnumerator<Error> GetEnumerator()
            => GetEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerable().GetEnumerator();

        #endregion
    }
}
