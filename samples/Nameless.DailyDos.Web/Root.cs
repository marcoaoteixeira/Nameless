namespace Nameless.DailyDos.Web {
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

        public static class Defaults {
            #region Internal Constants

            internal const string JWT_SECRET = "VGhlIG1vb24sIGEgY2VsZXN0aWFsIHBvZXQncyBwZWFybCwgYmF0aGVzIHRoZSBuaWdodCBjYW52YXMgaW4gYW4gZXRoZXJlYWwgZ2xvdywgd2hpc3BlcmluZyBhbmNpZW50IHNlY3JldHMgdG8gdGhlIHN0YXJnYXplcidzIHNvdWwsIGFuIGV0ZXJuYWwgZGFuY2Ugb2YgbGlnaHQgdGhhdCB3ZWF2ZXMgZHJlYW1zIGFjcm9zcyB0aGUgY29zbWljIHRhcGVzdHJ5Lg==";

            #endregion

            #region Internal Static Read-Only Properties

            internal static string[] OptionsSettingsTail = new[] { "Options", "Settings" };

            #endregion
        }

        #endregion

        #region Internal Static Inner Classes

        internal static class Endpoints {
            #region Internal Constants

            internal const string BASE_API_PATH = "api/v{version:apiVersion}";

            #endregion
        }

        #endregion
    }
}
