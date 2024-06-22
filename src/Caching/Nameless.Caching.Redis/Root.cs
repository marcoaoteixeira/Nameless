namespace Nameless.Caching.Redis {
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
        #region Public Static Inner Classes

        public static class EnvTokens {
            #region Public Static Read-Only Properties

            public const string REDIS_USER = nameof(REDIS_USER);
            public const string REDIS_PASS = nameof(REDIS_PASS);

            public const string REDIS_CERT_PASS = nameof(REDIS_CERT_PASS);

            #endregion
        }

        #endregion
    }
}
