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
        /// <returns>
        ///     A <see cref="SemanticVersion"/> representing the semantic
        ///     version.
        /// </returns>
        public SemanticVersion ToSemanticVersion() {
            return SemanticVersion.FromVersion(self);
        }
    }
}
