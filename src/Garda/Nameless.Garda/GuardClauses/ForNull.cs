using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Nameless {
    public static class ForNull {
        #region Public Static Methods

        [DebuggerStepThrough]
        public static T Null<T>(this IGuardClause _, [NotNull] T input, string name, string? message = null)
            => input ?? throw new ArgumentNullException(name, message ?? $"Argument {name} is null.");
    }

    #endregion
}