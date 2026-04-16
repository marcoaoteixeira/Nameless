namespace Nameless.Web;

public class WebDefaults {
    public static class CorsPolicies {
        public const string AllowEverything = "CP: Allow_Everything";
    }

    public static class OutputCachePolicies {
        public const string OneSecond = "OC: 1_Seconds";
        public const string FiveSeconds = "OC: 5_Seconds";
        public const string FifteenSeconds = "OC: 15_Seconds";
        public const string ThirtySeconds = "OC: 30_Seconds";
        public const string OneMinute = "OC: 1_Minute";
    }

    public static class RateLimiterPolicies {
        /// <summary>
        ///     A rate limiter policy that has a window of 60 seconds,
        ///     permits up to 10 requests and has 4 window segments.
        /// </summary>
        public const string SlidingWindow = "RL: Sliding_Window_60_Seconds_10_Requests_4_Segments";
    }

    public static class RequestTimeoutPolicies {
        public const string OneSecond = "RT: 1_Seconds";
        public const string FiveSeconds = "RT: 5_Seconds";
        public const string FifteenSeconds = "RT: 15_Seconds";
        public const string ThirtySeconds = "RT: 30_Seconds";
        public const string OneMinute = "RT: 1_Minute";
    }
}
