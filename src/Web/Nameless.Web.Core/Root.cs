﻿namespace Nameless.Web {
    /// <summary>
    /// This class was proposed to be a root point for this assembly.
    /// 
    /// *** DO NOT IMPLEMENT ANYTHING HERE ***
    /// 
    /// But, it's allowed to use it as a repository for all constants or
    /// default values that you'll use throughout this project.
    /// </summary>
    public static class Root {
        #region Public Static Inner Classes

        public static class HttpRequestHeaders {
            #region Public Constants

            public const string X_FORWARDED_FOR = "X-Forwarded-For";

            #endregion
        }

        public static class HttpResponseHeaders {
            #region Public Constants

            public const string X_JWT_EXPIRED = "X-JWT-Expired";

            #endregion
        }

        public static class EnvTokens {
            #region Public Constants

            public const string JWT_SECRET = nameof(JWT_SECRET);

            #endregion
        }

        public static class Defaults {
            #region Internal Constants

            internal const string JWT_SECRET = "VGhlIG1vb24sIGEgY2VsZXN0aWFsIHBvZXQncyBwZWFybCwgYmF0aGVzIHRoZSBuaWdodCBjYW52YXMgaW4gYW4gZXRoZXJlYWwgZ2xvdywgd2hpc3BlcmluZyBhbmNpZW50IHNlY3JldHMgdG8gdGhlIHN0YXJnYXplcidzIHNvdWwsIGFuIGV0ZXJuYWwgZGFuY2Ugb2YgbGlnaHQgdGhhdCB3ZWF2ZXMgZHJlYW1zIGFjcm9zcyB0aGUgY29zbWljIHRhcGVzdHJ5Lg==";

            #endregion
        }

        #endregion
    }
}
