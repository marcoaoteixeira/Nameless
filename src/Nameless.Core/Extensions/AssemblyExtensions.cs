using System.Reflection;

namespace Nameless;

/// <summary>
///     <see cref="Assembly" /> extension methods.
/// </summary>
public static class AssemblyExtensions {
    /// <param name="self">
    ///     The current assembly.
    /// </param>
    extension(Assembly self) {
        /// <summary>
        ///     Retrieves the assembly directory path.
        /// </summary>
        public string GetDirectoryPath() {
            var location = $"file://{self.Location}";
            var uri = new UriBuilder(location);
            var filePath = Uri.UnescapeDataString(uri.Path);

            return Path.GetDirectoryName(filePath) ?? string.Empty;
        }

        /// <summary>
        ///     Retrieves the semantic name of the assembly.
        /// </summary>
        /// <returns>
        ///     A string containing the semantic name of the assembly.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     Thrown if the assembly does not have a name.
        /// </exception>
        /// <remarks>
        ///     Under normal circumstances, this exception should never be
        ///     thrown, as every assembly is expected to have a name.
        /// </remarks>
        public string GetSemanticName() {
            return self.GetName().Name
                   ?? throw new InvalidOperationException("Assembly has no name.");
        }

        /// <summary>
        ///     Retrieves the semantic version for the current assembly.
        ///     See <a href="https://semver.org/">Semantic Versioning</a>.
        /// </summary>
        /// <returns>The semantic version.</returns>
        public string GetSemanticVersion() {
            var version = self.GetName()
                              .Version ?? new Version(major: 0, minor: 0, build: 0);

            return $"v{version.Major}.{version.Minor}.{version.Build}";
        }

        /// <summary>
        ///     Searches for all concrete implementations of a given service
        ///     <paramref name="service" />.
        /// </summary>
        /// <param name="service">
        ///     The service type.
        /// </param>
        /// <returns>
        ///     A collection of types that implements <paramref name="service" />.
        /// </returns>
        /// <remarks>
        ///     We look for all exported types in the assembly that are not pointer
        ///     types, not by-ref types, not abstract, and are public. Then we
        ///     check if the type is assignable to the service type or if it is a
        ///     generic type that is assignable to the service type.
        /// </remarks>
        public IEnumerable<Type> GetImplementations(Type service) {
            // retrieve all exported types from the assembly
            // that are relevant to us
            return self.GetExportedTypes()
                       .Where(type => type is {
                           // Exclude pointer types
                           IsPointer: false,

                           // Exclude by-ref types
                           IsByRef: false,

                           // Exclude abstract types (can't be instantiated)
                           IsAbstract: false,

                           // Only public types
                           IsPublic: true
                       })
                       // inside the types, we will look for all types that
                       // are assignable to the service type
                       .Where(type => service.IsAssignableFrom(type) ||
                                      service.IsAssignableFromGenericType(type));
        }
    }

    /// <summary>
    ///     Searches for all concrete implementations of all given services
    ///     <paramref name="services" />.
    /// </summary>
    /// <param name="services">
    ///     The service type.
    /// </param>
    /// <param name="self">
    ///     The current collection of <see cref="Assembly"/>.
    /// </param>
    /// <returns>
    ///     A collection of types that implements any of
    ///     <paramref name="services" />.
    /// </returns>
    public static IEnumerable<Type> GetImplementations(this IEnumerable<Assembly> self, params IEnumerable<Type> services) {
        return
            from assembly in self
            from service in services
            from implementation in assembly.GetImplementations(service)
            select implementation;
    }
}