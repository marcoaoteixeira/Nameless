using System.Diagnostics;

namespace Nameless.Diagnostics;

/// <summary>
///     Represents an abstraction for an activity source.
/// </summary>
public interface IActivitySource : IDisposable {
    /// <summary>
    ///     Gets the activity source name.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Gets the activity source version.
    /// </summary>
    string? Version { get; }

    /// <summary>
    ///     Creates and starts a new <see cref="IActivity"/> object if there is
    ///     any listener to the Activity events, returns
    ///     <see cref="NullActivity.Instance"/> otherwise.
    /// </summary>
    /// <param name="name">
    ///     The operation name of the Activity.
    /// </param>
    /// <param name="kind">
    ///     The <see cref="ActivityKind"/>
    /// </param>
    /// <param name="parentContext">
    ///     The parent <see cref="ActivityContext"/> object to initialize the
    ///     created Activity object with.
    /// </param>
    /// <returns>
    ///     The created <see cref="IActivity"/> object or
    ///     <see cref="NullActivity.Instance"/> if there is no any listener.
    /// </returns>
    IActivity StartActivity(string name, ActivityKind kind, ActivityContext? parentContext);
}