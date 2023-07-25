using System.Collections;

namespace Nameless.ErrorHandling {
    public sealed class ErrorCollection : IEnumerable<Error> {
        #region Public Static Read-Only Properties

        public static ErrorCollection Empty => new();

        #endregion

        #region Private Read-Only Properties

        private Dictionary<string, ISet<string>> Errors { get; } = new(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region Public Methods

        public void Push(string code, params string[] problems) {
            Prevent.Against.Null(code, nameof(code));
            Prevent.Against.Null(problems, nameof(problems));

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
            if (!Errors.ContainsKey(code)) {
                Errors.Add(code, new HashSet<string>());
            }
            return Errors[code];
        }

        private IEnumerable<Error> GetEnumerable() {
            foreach (var item in Errors) {
                yield return new(item.Key, item.Value.ToArray());
            }
        }

        #endregion

        #region IEnumerable<Error> Members

        IEnumerator<Error> IEnumerable<Error>.GetEnumerator()
            => GetEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerable().GetEnumerator();

        #endregion
    }
}
