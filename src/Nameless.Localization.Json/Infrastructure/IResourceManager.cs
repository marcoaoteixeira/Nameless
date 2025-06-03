using Nameless.Localization.Json.Objects;

namespace Nameless.Localization.Json.Infrastructure;

/// <summary>
///     Provides methods to deal with JSON resource files.
/// </summary>
public interface IResourceManager {
    /// <summary>
    ///     Retrieves a resource given its path and associated culture.
    /// </summary>
    /// <param name="baseName">The base path to the resource.</param>
    /// <param name="location">The location of the resource.</param>
    /// <param name="culture">The culture of the resource.</param>
    /// <returns>An instance of <see cref="Resource" />.</returns>
    Resource GetResource(string baseName, string location, string culture);
}