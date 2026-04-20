namespace Nameless.ObjectModel;

/// <summary>
///     <see cref="Error"/> extension methods.
/// </summary>
public static class ErrorExtensions {
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
    }
}
