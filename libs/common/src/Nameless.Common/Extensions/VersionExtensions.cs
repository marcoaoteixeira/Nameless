using Nameless.ObjectModel;

namespace Nameless;

/// <summary>
///     <see cref="Version"/> extension methods
/// </summary>
public static class VersionExtensions {
    /// <param name="self">
    ///     The current <see cref="Version"/> instance.
    /// </param>
    extension(Version self) {
        /// <summary>
        ///     Retrieves the semantic representation of the version.
        /// </summary>
        /// <param name="prefix">
        ///     The version prefix.
        /// </param>
        /// <returns>
        ///     A <see cref="SemVersion"/> representing the semantic
        ///     version.
        /// </returns>
        public SemVersion ToSemVersion(char? prefix = null) {
            return SemVersion.FromVersion(self, prefix);
        }
    }
}
