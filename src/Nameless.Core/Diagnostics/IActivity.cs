using System.Diagnostics;

namespace Nameless.Diagnostics;

/// <summary>
///     An abstraction for an activity.
/// </summary>
public interface IActivity : IDisposable {
    /// <summary>
    ///     <para>Add or update the Activity tag with the input key and value.</para>
    ///     <list type="bullet">
    ///         <item>
    ///             <term>If the input value is null</term>
    ///             <description>
    ///                 <para>
    ///                     - if the collection has any tag with the same key,
    ///                     then this tag will get removed from the collection.
    ///                 </para>
    ///                 <para>
    ///                     - otherwise, nothing will happen and the collection
    ///                     will not change.
    ///                 </para>
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <term>If the input value is not null</term>
    ///             <description>
    ///                 <para>
    ///                     - if the collection has any tag with the same key,
    ///                     then the value mapped to this key will get updated
    ///                     with the new input value.
    ///                 </para>
    ///                 <para>
    ///                     - otherwise, the key and value will get added as a
    ///                     new tag to the collection.
    ///                 </para>
    ///             </description>
    ///         </item>
    ///     </list>
    /// </summary>
    /// <param name="key">
    ///     The tag key name
    /// </param>
    /// <param name="value">
    ///     The tag value mapped to the input key
    /// </param>
    /// <returns>
    ///     The current <see cref="IActivity"/> instance so other actions can
    ///     be chained.
    /// </returns>
    IActivity SetTag(string key, object? value);

    /// <summary>
    ///     Add an <see cref="ActivityEvent" /> object containing the exception
    ///     information to the events list.
    /// </summary>
    /// <param name="exception">
    ///     The exception to add to the attached events list.
    /// </param>
    /// <returns>
    ///     The current <see cref="IActivity"/> instance so other actions can
    ///     be chained.
    /// </returns>
    IActivity AddException(Exception exception);

    /// <summary>
    ///     Sets the status code and description on the current activity
    ///     object.
    /// </summary>
    /// <param name="code">
    ///     The status code
    /// </param>
    /// <param name="description">
    ///     The status code description.
    /// </param>
    /// <returns>
    ///     The current <see cref="IActivity"/> instance so other actions can
    ///     be chained.
    /// </returns>
    IActivity SetStatus(ActivityStatusCode code, string? description);
}