using System.Reflection;

namespace Nameless;

/// <summary>
///     <see cref="Assembly" /> extension methods.
/// </summary>
public static class AssemblyExtensions {
    /// <summary>
    ///     Retrieves the assembly directory path.
    /// </summary>
    /// <param name="self">The current assembly.</param>
    public static string GetDirectoryPath(this Assembly self) {
        var location = $"file://{self.Location}";
        var uri = new UriBuilder(location);
        var filePath = Uri.UnescapeDataString(uri.Path);

        return Path.GetDirectoryName(filePath) ?? string.Empty;
    }

    /// <summary>
    ///     Retrieves the semantic version for the current assembly.
    ///     See <a href="https://semver.org/">Semantic Versioning</a>.
    /// </summary>
    /// <param name="self">The current assembly.</param>
    /// <returns>The semantic version.</returns>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="self" /> is <see langword="null"/>.
    /// </exception>
    public static string GetSemanticVersion(this Assembly self) {
        var version = self.GetName().Version ?? new Version(0, 0, 0);

        return $"v{version.Major}.{version.Minor}.{version.Build}";
    }

    /// <summary>
    ///     Searches for all concrete implementations of all given services <paramref name="services" />
    /// </summary>
    /// <param name="self">The current collection of <see cref="Assembly"/>.</param>
    /// <param name="services">The service type.</param>
    /// <returns>
    ///     A collection of types that implements any of <paramref name="services" />.
    /// </returns>
    public static IEnumerable<Type> GetImplementations(this IEnumerable<Assembly> self, params IEnumerable<Type> services) {
        return from assembly in self
               from service in services
               from implementation in assembly.GetImplementations(service)
               select implementation;
    }

    /// <summary>
    ///     Searches for all concrete implementations of a given service <paramref name="service" />
    /// </summary>
    /// <param name="self">The current <see cref="Assembly"/>.</param>
    /// <param name="service">The service type.</param>
    /// <returns>
    ///     A collection of types that implements <paramref name="service" />.
    /// </returns>
    /// <remarks>
    /// We look for all exported types in the assembly that are not pointer types,
    /// not by-ref types, not abstract, and are public. Then we check if the type
    /// is assignable to the service type or if it is a generic type that is assignable
    /// to the service type.
    /// </remarks>
    public static IEnumerable<Type> GetImplementations(this Assembly self, Type service) {
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