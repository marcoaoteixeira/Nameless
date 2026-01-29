using System.Diagnostics;

namespace Nameless.Diagnostics;

/// <summary>
///     A wrapper for <see cref="ActivitySource"/> that implements
///     some interface abstraction.
/// </summary>
public class ActivitySourceWrapper : IActivitySource {
    private readonly ActivitySource _activitySource;

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="ActivitySourceWrapper"/> class.
    /// </summary>
    /// <param name="activitySource">
    ///     The current activity source.
    /// </param>
    public ActivitySourceWrapper(ActivitySource activitySource) {
        _activitySource = activitySource;
    }

    /// <inheritdoc />
    public IActivity StartActivity(string name, ActivityKind kind, ActivityContext? parentContext) {
        var activity = parentContext.HasValue
            ? _activitySource.StartActivity(name, kind, parentContext.Value)
            : _activitySource.StartActivity(name, kind);

        return activity is not null
            ? new ActivityWrapper(activity)
            : NullActivity.Instance;
    }
}