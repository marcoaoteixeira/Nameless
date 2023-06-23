using System.Globalization;

namespace Nameless {
    /// <summary>
    /// <see cref="CultureInfo"/> extension methods.
    /// </summary>
    public static class CultureInfoExtension {
        #region Public Static Methods

        /// <summary>
        /// Retrieves the culture tree.
        /// </summary>
        /// <param name="self">The current culture info.</param>
        /// <returns>An instance of <see cref="IEnumerable{CultureInfo}"/> with all lower cultures.</returns>
        /// <exception cref="NullReferenceException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static IEnumerable<CultureInfo> GetTree(this CultureInfo self) {
            var currentCulture = new CultureInfo(self.Name);
            while (!string.IsNullOrWhiteSpace(currentCulture.Name)) {
                yield return currentCulture;
                currentCulture = currentCulture.Parent;
            }
        }

        #endregion
    }
}