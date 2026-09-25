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
        /// <param name="ex">
        ///     The operation canceled exception.
        /// </param>
        /// <returns>
        ///     An <see cref="Error"/> (Conflict) instance.
        /// </returns>
        public static Error OperationCanceled(OperationCanceledException? ex = null) {
            return Error.Failure("Operation was canceled unexpectedly.", ex: ex);
        }
    }

    extension(IEnumerable<Error> self) {
        /// <summary>
        ///     Folds all errors into a single error, by type.
        /// </summary>
        /// <param name="type">
        ///     Type of the error.
        /// </param>
        /// <returns>
        ///     A single error.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     if no errors are available.
        /// </exception>
        public Error AggregateByType(ErrorType type) {
            var errors = self.Where(error => error.Type == type).ToArray();

            switch (errors.Length) {
                case 0: throw new InvalidOperationException(
                    $"No errors of type '{type}' to aggregate."
                );
                case 1: return errors[0];
            }

            var codes = errors.Select(error => error.Code)
                              .Where(code => !string.IsNullOrWhiteSpace(code))
                              .ToArray();

            var exceptions = errors.Select(error => error.Exception)
                                   .OfType<Exception>()
                                   .ToArray();

            const string Separator = "; ";

            return new Error(
                message: string.Join(Separator, errors.Select(error => error.Message)),
                code: codes.Length > 0 ? string.Join(Separator, codes) : null,
                type: type,
                ex: exceptions.Length switch {
                    0 => null,
                    1 => exceptions[0],
                    _ => new AggregateException(exceptions)
                }
            );
        }

        /// <summary>
        ///     Retrieves a string representation of the error collection.
        /// </summary>
        /// <returns>
        ///     A string representing all errors separated by semicolon.
        /// </returns>
        public string Flatten(char separator = '|') {
            var items = self.GroupBy(error => error.Type).Select(Flat);

            return string.Join($" {separator} ", items);

            static string Flat(IGrouping<ErrorType, Error> group) {
                var messages = group.Select(item => !string.IsNullOrWhiteSpace(item.Code)
                    ? $"({item.Code}) {item.Message}"
                    : item.Message
                );

                return $"[{group.Key}] {string.Join("; ", messages)}";
            }
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
