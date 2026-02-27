namespace Nameless.Web;

public class Defaults {
    public static class CorsPolicies {
        public const string ALLOW_EVERYTHING = "CP: Allow_Everything";
    }

    public static class OutputCachePolicies {
        public const string ONE_SECOND = "OC: 1_Seconds";
        public const string FIVE_SECONDS = "OC: 5_Seconds";
        public const string FIFTEEN_SECONDS = "OC: 15_Seconds";
        public const string THIRTY_SECONDS = "OC: 30_Seconds";
        public const string ONE_MINUTE = "OC: 1_Minute";
    }

    public static class RateLimiterPolicies {
        /// <summary>
        ///     A rate limiter policy that has a window of 60 seconds,
        ///     permits up to 10 requests and has 4 window segments.
        /// </summary>
        public const string SLIDING_WINDOW = "RL: Sliding_Window_60_Seconds_10_Requests_4_Segments";
    }

    public static class RequestTimeoutPolicies {
        public const string ONE_SECOND = "RT: 1_Seconds";
        public const string FIVE_SECONDS = "RT: 5_Seconds";
        public const string FIFTEEN_SECONDS = "RT: 15_Seconds";
        public const string THIRTY_SECONDS = "RT: 30_Seconds";
        public const string ONE_MINUTE = "RT: 1_Minute";
    }
}
