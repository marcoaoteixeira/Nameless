using System.Diagnostics;

namespace Nameless {
    public static class ForExpression {
        #region Public Static Methods

        [DebuggerStepThrough]
        public static T Expression<T>(this Prevent _, Func<T, bool> expression, T input, string? message = null) {
            if (!expression(input)) {
                throw new FalseOutcomeException(message ?? "Expression evaluated to false.");
            }
            return input;
        }

        #endregion
    }
}
