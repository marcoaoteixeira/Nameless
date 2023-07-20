using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Nameless {
    public static class ForRegexp {
        #region Public Static Methods

        [DebuggerStepThrough]
        public static string NoMatchingPattern(this Prevent _, string input, string name, string pattern, string? message = null) {
            var match = Regex.Match(input, pattern);
            if (!match.Success || match.Value != input) {
                throw new ArgumentException(message ?? $"Argument {name} does not match pattern {pattern}", name);
            }
            return input;
        }

        #endregion
    }
}
