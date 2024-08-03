using System.Reflection;

namespace Nameless {
    /// <summary>
    /// <see cref="Assembly"/> extension methods.
    /// </summary>
    public static class AssemblyExtension {
        #region Public Static Methods

        /// <summary>
        /// Retrieves the assembly directory path.
        /// </summary>
        /// <param name="self">The current assembly.</param>
        /// <param name="combineWith">Parts to concatenate with the result directory path.</param>
        /// <returns>The path to the assembly folder.</returns>
        /// <exception cref="NullReferenceException">if <paramref name="self"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">if <paramref name="combineWith"/> is <c>null</c>.</exception>
        public static string GetDirectoryPath(this Assembly self, params string[] combineWith) {
            Guard.Against.Null(combineWith, nameof(combineWith));

            var location = $"file://{self.Location}";
            var uri = new UriBuilder(location);
            var path = Uri.UnescapeDataString(uri.Path);

            var result = Path.GetDirectoryName(path)!;

            return combineWith.IsNullOrEmpty()
                ? result
                : Path.Combine(combineWith.Prepend(result).ToArray());
        }

        public static string GetSemanticVersion(this Assembly self) {
            var version = self.GetName().Version;

            return version is not null
                ? $"v{version.Major}.{version.Minor}.{version.Build}"
                : "v0.0.0";
        }

        /// <summary>
        /// Retrieves all types that implements <see cref="TBase"/>.
        /// </summary>
        /// <typeparam name="TBase">The base implementation type.</typeparam>
        /// <param name="self">The current assembly object.</param>
        /// <returns>A collection of types.</returns>
        public static IEnumerable<Type> ScanAssembly<TBase>(this Assembly self)
            => ScanAssembly(self, typeof(TBase));

        /// <summary>
        /// Retrieves all types that implements <paramref name="baseType"/>.
        /// </summary>
        /// <param name="self">The current assembly object.</param>
        /// <param name="baseType">The base type.</param>
        /// <returns>A collection of types.</returns>
        public static IEnumerable<Type> ScanAssembly(this Assembly self, Type baseType)
            => self.GetExportedTypes()
                   .Where(type => type is { IsInterface: false, IsAbstract: false } &&
                                  (baseType.IsAssignableFrom(type) || baseType.IsAssignableFromGenericType(type)));

        #endregion
    }
}