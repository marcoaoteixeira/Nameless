namespace Nameless.Localization.Microsoft.Json.Options {
    public sealed class LocalizationOptions {
        #region Public Static Read-Only Properties

        public static LocalizationOptions Default => new();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the folder that will be the place
        /// to look for translations. Relative to the application base path.
        /// </summary>
        public string TranslationFolderName { get; set; } = "Localization";
        /// <summary>
        /// Gets or sets whether it will watch the translation files for changes
        /// and reload if necessary.
        /// </summary>
        public bool WatchFileForChanges { get; set; } = true;

        #endregion
    }
}
