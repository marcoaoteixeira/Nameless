using System.Diagnostics;

namespace Nameless.Microservices.Application.Monitoring;

/// <summary>
///     Provides methods to retrieve <see cref="ActivitySource" /> instances.
/// </summary>
public interface IActivitySourceProvider {
    /// <summary>
    ///     Retrieves an <see cref="ActivitySource" /> instance with the specified name, version, and tags.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="version">The version.</param>
    /// <param name="tags">The tags.</param>
    /// <returns>
    ///     The <see cref="ActivitySource" /> instance.
    /// </returns>
    ActivitySource GetActivitySource(string name, string? version = null, IDictionary<string, object?>? tags = null);
}