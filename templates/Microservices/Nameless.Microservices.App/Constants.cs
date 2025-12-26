namespace Nameless.Microservices.App;

internal static class Constants {
    internal static class Database {
        internal const string CONN_STR_NAME = "Sqlite";
    }

    internal static class CorsPolicies {
        internal const string ALLOW_EVERYTHING = "AllowEverything";
    }

    internal static class OutputCachePolicies {
        internal const string ONE_MINUTE = "OneMinute";
    }

    internal static class RateLimitPolicies {
        internal const string SLIDING_WINDOW = "SlidingWindow";
    }

    internal static class RequestTimeoutPolicies {
        internal const string ONE_SECOND = "OneSecond";
        internal const string FIVE_SECONDS = "FiveSeconds";
        internal const string FIFTEEN_SECONDS = "FifteenSeconds";
        internal const string THIRTY_SECONDS = "ThirtySeconds";
        internal const string ONE_MINUTE = "OneMinute";
    }
}
