using System.Diagnostics;

namespace Nameless.Diagnostics;

/// <summary>
///     <see cref="IActivity"/> extension methods.
/// </summary>
public static class ActivityExtensions {
    /// <param name="self">
    ///     The current <see cref="IActivity"/> instance.
    /// </param>
    extension(IActivity self) {
        /// <summary>
        ///     Sets the status code and description on the current activity
        ///     object.
        /// </summary>
        /// <param name="code">
        ///     The status code
        /// </param>
        /// <returns>
        ///     The current <see cref="IActivity"/> instance so other actions can
        ///     be chained.
        /// </returns>
        public IActivity SetStatus(ActivityStatusCode code) {
            return self.SetStatus(code, description: null);
        }
    }
}