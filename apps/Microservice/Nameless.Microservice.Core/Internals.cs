namespace Nameless.Microservice {
    internal static class Internals {
        #region Internal Static Inner Classes

        internal static class HttpResponseHeaders {
            #region Internal Constants

            internal const string JSON_WEB_TOKEN_EXPIRED = "X-JWT-Expired";

            #endregion
        }

        internal static class EnvironmentTokens {
            #region Internal Constants

            internal const string JWT_SECRET = "JWT_SECRET";

            #endregion
        }

        internal static class ClassTokens {
            #region Internal Constants

            internal const string OPTIONS = "Options";
            internal const string SETTINGS = "Settings";

            #endregion
        }

        internal static class Secrets {
            #region Internal Static Properties

            internal static readonly string Jwt = "VGhlIG1vb24sIGEgY2VsZXN0aWFsIHBvZXQncyBwZWFybCwgYmF0aGVzIHRoZSBuaWdodCBjYW52YXMgaW4gYW4gZXRoZXJlYWwgZ2xvdywgd2hpc3BlcmluZyBhbmNpZW50IHNlY3JldHMgdG8gdGhlIHN0YXJnYXplcidzIHNvdWwsIGFuIGV0ZXJuYWwgZGFuY2Ugb2YgbGlnaHQgdGhhdCB3ZWF2ZXMgZHJlYW1zIGFjcm9zcyB0aGUgY29zbWljIHRhcGVzdHJ5Lg==";

            #endregion
        }

        #endregion
    }
}
