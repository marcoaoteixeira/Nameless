namespace Nameless.Web.RequestTimeouts;

public class RequestTimeoutPolicyOptions {
    public RequestTimeoutPolicy[] Entries { get; set; } = [
        RequestTimeoutPolicy.OneSecond,
        RequestTimeoutPolicy.FiveSeconds,
        RequestTimeoutPolicy.FifteenSeconds,
        RequestTimeoutPolicy.ThirtySeconds,
        RequestTimeoutPolicy.OneMinute,
    ];
}