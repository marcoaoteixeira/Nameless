namespace Nameless.ObjectModel;

/// <summary>
///     <see cref="Error"/> extension methods.
/// </summary>
public static class ErrorExtensions {
    extension(Error) {
        /// <summary>
        ///     Creates an <see cref="Error"/> informing about the current
        ///     operation cancellation. Message defaults to
        ///     <c>Operation was canceled unexpectedly.</c>
        /// </summary>
        /// <param name="exception">
        ///     The operation canceled exception.
        /// </param>
        /// <returns>
        ///     An <see cref="Error"/> (Conflict) instance.
        /// </returns>
        public static Error OperationCanceled(OperationCanceledException? exception = null) {
            return Error.Failure("Operation was canceled unexpectedly.", exception: exception);
        }
    }

    extension(IEnumerable<Error> self) {
        /// <summary>
        ///     Retrieves a string representation of the error collection.
        /// </summary>
        /// <returns>
        ///     A string representing all errors separated by semicolon.
        /// </returns>
        public string Flatten() {
            return string.Join("; ", self.Select(error => error.Flatten));
        }

        /// <summary>
        ///     Creates a <see cref="Dictionary{TKey,TValue}"/> from the
        ///     errors where the key is the <see cref="Error.Code"/> property
        ///     and the values are the <see cref="Error.Message"/>, grouped
        ///     by the <see cref="Error.Code"/>.
        /// </summary>
        /// <returns>
        ///     An instance of <see cref="Dictionary{TKey,TValue}"/> where the
        ///     key is the <see cref="Error.Code"/> property and the values
        ///     are the <see cref="Error.Message"/>, grouped by the
        ///     <see cref="Error.Code"/>.
        /// </returns>
        public IDictionary<string, string[]> ToDictionary() {
            return self.GroupBy(error => error.Code).ToDictionary(
                keySelector: group => group.Key ?? string.Empty,
                elementSelector: group => group.Select(item => item.Message).ToArray()
            );
        }
    }
}
