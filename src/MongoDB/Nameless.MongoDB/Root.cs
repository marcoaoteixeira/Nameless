namespace Nameless.MongoDB {
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

            public const string MONGO_CRED_DATABASE = nameof(MONGO_CRED_DATABASE);
            public const string MONGO_CRED_MECHANISM = nameof(MONGO_CRED_MECHANISM);
            public const string MONGO_CRED_USER = nameof(MONGO_CRED_USER);
            public const string MONGO_CRED_PASS = nameof(MONGO_CRED_PASS);

            #endregion
        }

        #endregion

        #region Internal Static Inner Classes

        internal static class Defaults {
            #region Internal Static Read-Only Properties

            internal const string MONGO_CRED_DATABASE = "admin";
            internal const string MONGO_CRED_MECHANISM = "SCRAM-SHA-256";
            internal const string MONGO_CRED_USER = "root";
            internal const string MONGO_CRED_PASS = "root";

            #endregion
        }

        #endregion
    }
}
