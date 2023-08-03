namespace Nameless.Localization.Microsoft.Json {
    public sealed class LocalizationOptions {
        #region Public Static Read-Only Properties

        public static LocalizationOptions Default => new();

        #endregion

        #region Public Constructors

        public LocalizationOptions() {
            TranslationFolder = "App_Data/Localization/";
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the relative path to the translation files folder.
        /// </summary>
        public string TranslationFolder { get; set; }
        /// <summary>
        /// Gets or sets whether will watch the translation files for changes
        /// and reload if necessary.
        /// </summary>
        public bool WatchFileForChanges { get; set; } = true;

        #endregion
    }
}
