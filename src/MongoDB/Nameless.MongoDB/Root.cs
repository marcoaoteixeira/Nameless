namespace Nameless.MongoDB {
    /// <summary>
    /// The only purpose of this class is to be an "entrypoint" for this
    /// assembly.
    /// 
    /// *** DO NOT IMPLEMENT ANYTHING HERE ***
    /// 
    /// But, it's OK to use it as a repository for all constants or default
    /// values that you'll use throughout this assembly.
    /// </summary>
    public static class Root {
        #region Internal Static Inner Classes

        internal static class Defaults {
            #region Internal Static Read-Only Properties

            internal const string MONGO_HOST = "localhost";
            internal const int MONGO_PORT = 27017;
            internal const string MONGO_CRED_DATABASE = "admin";
            internal const string MONGO_CRED_MECHANISM = "SCRAM-SHA-256";
            internal const string MONGO_CRED_USER = "root";
            internal const string MONGO_CRED_PASS = "root";

            #endregion
        }

        #endregion
    }
}
