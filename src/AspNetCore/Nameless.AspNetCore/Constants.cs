namespace Nameless.AspNetCore {
    public static class Constants {
        #region Public Static Inner Classes

        public static class HttpRequestHeaders {
            #region Public Constants

            public const string X_FORWARDED_FOR = "X-Forwarded-For";
            public const string X_FORWARDED_PROTO = "X-Forwarded-Proto";
            public const string X_FORWARDED_PORT = "X-Forwarded-Port";

            #endregion
        }

        public static class HttpResponseHeaders {
            #region Public Constants

            public const string JSON_WEB_TOKEN_EXPIRED = "X-JWT-Expired";

            #endregion
        }

        public static class ClassTokens {
            #region Public Constants

            public const string OPTIONS = "Options";
            public const string SETTINGS = "Settings";

            #endregion
        }

        public static class EnvironmentTokens {
            #region Public Constants

            public const string JWT_SECRET = "JWT_SECRET";

            #endregion
        }

        #endregion
    }
}
