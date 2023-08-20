﻿namespace Nameless.Caching.Redis {
    /// <summary>
    /// This class was defined to be an entrypoint for this project assembly.
    /// 
    /// *** DO NOT IMPLEMENT ANYTHING HERE ***
    /// 
    /// But, it's allow to use this class as a repository for all constants or
    /// default values that we'll use throughout this project.
    /// </summary>
    public static class Root {
        #region Public Static Inner Classes

        public static class EnvTokens {
            #region Public Static Read-Only Properties

            public const string REDIS_USER = nameof(REDIS_USER);
            public const string REDIS_PASS = nameof(REDIS_PASS);
            public const string REDIS_CERT_PFX = nameof(REDIS_CERT_PFX);
            public const string REDIS_CERT_PEM = nameof(REDIS_CERT_PEM);
            public const string REDIS_CERT_PASS = nameof(REDIS_CERT_PASS);

            #endregion
        }

        #endregion

        #region Internal Static Inner Classes

        internal static class Defaults {
            #region Internal Static Read-Only Properties

            internal const string REDIS_USER = "root";
            internal const string REDIS_PASS = "123456@AbC";

            #endregion
        }

        #endregion
    }
}
