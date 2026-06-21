using System.Diagnostics;

namespace Nameless.Diagnostics;

/// <summary>
///     <see cref="IActivitySource"/> method extensions.
/// </summary>
public static class ActivitySourceExtensions {
    extension(IActivitySource self) {
        /// <summary>
        ///     Starts a new activity.
        /// </summary>
        /// <param name="name">
        ///     The name of the activity to start.
        /// </param>
        /// <returns>
        ///     An <see cref="IActivity"/> instance that represents the
        ///     started activity, or <see langword="null"/> if the activity
        ///     is not created due to sampling or configuration.
        /// </returns>
        public IActivity StartActivity(string name) {
            return self.StartActivity(name, ActivityKind.Internal, parentContext: null);
        }

        /// <summary>
        ///     Starts a new activity.
        /// </summary>
        /// <param name="name">
        ///     The name of the activity to start.
        /// </param>
        /// <param name="kind">
        ///     The kind of activity to start.
        /// </param>
        /// <returns>
        ///     An <see cref="IActivity"/> instance that represents the
        ///     started activity, or <see langword="null"/> if the activity
        ///     is not created due to sampling or configuration.
        /// </returns>
        public IActivity StartActivity(string name, ActivityKind kind) {
            return self.StartActivity(name, kind, parentContext: null);
        }
    }
}