using System.Diagnostics;

namespace Nameless.Microservices.Application.Monitoring;

/// <summary>
///     Represents a manager of <see cref="ActivitySource" /> instances for distributed tracing.
/// </summary>
public interface IActivitySourceManager {
    /// <summary>
    ///     Creates an <see cref="ActivitySource" /> instance with the specified version, and tags.
    /// </summary>
    /// <param name="sourceName">The source name.</param>
    /// <param name="version">The version.</param>
    /// <param name="tags">The tags.</param>
    /// <returns>
    ///     The <see cref="ActivitySource" /> instance.
    /// </returns>
    ActivitySource GetActivitySource(string sourceName, string? version = null, IDictionary<string, object?>? tags = null);

    /// <summary>
    ///     Removes the active <see cref="ActivitySource" /> for the specified type and version.
    /// </summary>
    /// <param name="sourceName">The source name.</param>
    /// <param name="version">The version.</param>
    /// <returns>
    ///     <see langword="true"/> if the <see cref="ActivitySource" /> was successfully removed; otherwise, <see langword="false"/>.
    /// </returns>
    bool RemoveActiveSource(string sourceName, string? version = null);
}