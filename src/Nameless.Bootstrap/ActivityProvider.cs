using System.Diagnostics;
using Nameless.Diagnostics;

namespace Nameless.Bootstrap;

public sealed class ActivityProvider {
    private readonly ActivitySource _activitySource = new(typeof(ActivityProvider).Assembly.GetSemanticName());

    public static ActivityProvider Instance { get; } = new();

    static ActivityProvider() { }

    private ActivityProvider() { }

    public IActivity StartActivity(string name, ActivityKind kind, ActivityContext? parentContext) {
        var activity = parentContext is not null
            ? _activitySource.StartActivity(name, kind, parentContext.Value)
            : _activitySource.StartActivity(name, kind);

        return activity is not null
            ? new ActivityWrapper(activity)
            : NullActivity.Instance;
    }
}
