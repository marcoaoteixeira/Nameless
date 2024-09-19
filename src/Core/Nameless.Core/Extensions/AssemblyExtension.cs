using System.Reflection;
using Nameless.Helpers;

namespace Nameless;

/// <summary>
/// <see cref="Assembly"/> extension methods.
/// </summary>
public static class AssemblyExtension {
    /// <summary>
    /// Retrieves the assembly directory path.
    /// </summary>
    /// <param name="self">The current assembly.</param>
    /// <param name="combineWith">Parts to concatenate with the result directory path.</param>
    /// <returns>The path to the assembly folder.</returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> or
    /// <paramref name="combineWith"/> is <c>null</c>.
    /// </exception>
    public static string GetDirectoryPath(this Assembly self, params string[] combineWith) {
        Prevent.Argument.Null(self);
        Prevent.Argument.Null(combineWith);

        var location = $"file://{self.Location}";
        var uri = new UriBuilder(location);
        var filePath = Uri.UnescapeDataString(uri.Path);
        var directoryPath = Path.GetDirectoryName(filePath);

        if (directoryPath is null) { return string.Empty; }

        var result = Path.Combine(combineWith.Prepend(directoryPath)
                                             .ToArray());
        
        return PathHelper.Normalize(result);
    }

    /// <summary>
    /// Retrieves the semantic version for the current assembly.
    /// See <a href="https://semver.org/">Semantic Versioning</a>.
    /// </summary>
    /// <param name="self">The current assembly.</param>
    /// <returns>The semantic version.</returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>.
    /// </exception>
    public static string GetSemanticVersion(this Assembly self) {
        Prevent.Argument.Null(self);

        var version = self.GetName()
                          .Version ?? new Version(0, 0, 0);
        return $"v{version.Major}.{version.Minor}.{version.Build}";
    }

    /// <summary>
    /// Searches for all implementations of a given service <typeparamref name="TService"/>
    /// in the current assembly.
    /// <br /><br />
    /// <strong>Note:</strong> we look only for public exported types, never private, protected or internal.
    /// </summary>
    /// <typeparam name="TService">The type of the service, usually the base class or interface.</typeparam>
    /// <param name="self">The current assembly.</param>
    /// <returns>A collection of types that implements <typeparamref name="TService"/>.</returns>
    public static IEnumerable<Type> SearchForImplementations<TService>(this Assembly self)
        => SearchForImplementations(self, typeof(TService));

    /// <summary>
    /// Searches for all implementations of a given service <paramref name="serviceType"/>
    /// in the current assembly.
    /// <br /><br />
    /// <strong>Note:</strong> we look only for public exported types, never private, protected or internal.
    /// </summary>
    /// <param name="self">The current assembly.</param>
    /// <param name="serviceType">The service type.</param>
    /// <returns>A collection of types that implements <paramref name="serviceType"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> or
    /// <paramref name="serviceType"/> is <c>null</c>.
    /// </exception>
    public static IEnumerable<Type> SearchForImplementations(this Assembly self, Type serviceType) {
        Prevent.Argument.Null(self);
        Prevent.Argument.Null(serviceType);

        return self.GetExportedTypes()
                   .Where(type => type is { IsInterface: false, IsAbstract: false } &&
                                  type.GetCustomAttribute<SingletonAttribute>(inherit: true) is null &&
                                  (serviceType.IsAssignableFrom(type) || serviceType.IsAssignableFromOpenGenericType(type)));
    }
}