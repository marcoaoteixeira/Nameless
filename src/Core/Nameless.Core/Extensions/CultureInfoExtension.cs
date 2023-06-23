using System.Globalization;

namespace Nameless {

    /// <summary>
    /// Extension methods for <see cref="CultureInfo"/>.
    /// </summary>
    public static class CultureInfoExtension {

        #region Public Static Methods

        /// <summary>
        /// Retrieves the culture tree.
        /// </summary>
        /// <param name="self">The current culture info.</param>
        /// <returns>An instance of <see cref="IEnumerable{CultureInfo}"/> with all lower cultures.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static IEnumerable<CultureInfo> GetTree(this CultureInfo self) {
            Prevent.Null(self, nameof(self));

            var currentCulture = new CultureInfo(self.Name);
            while (!string.IsNullOrWhiteSpace(currentCulture.Name)) {
                yield return currentCulture;
                currentCulture = currentCulture.Parent;
            }
        }

        #endregion
    }
}