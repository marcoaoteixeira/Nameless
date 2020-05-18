namespace Nameless.Localization.Json {
    public class LocalizationSettings {
 
        #region Public Properties

        /// <summary>
        /// Gets or sets the localization resource folder path. The folder path
        /// must be relative to the application file storage root.
        /// </summary>
        public string ResourceFolderPath { get; set; } = "Localization";
        
        /// <summary>
        /// Gets or sets the default culture name.
        /// </summary>
        public string DefaultCultureName { get; set; } = "en-US";
        
        /// <summary>
        /// Whether it will watch localization resource files for changes and
        /// reload.
        /// </summary>
        public bool ReloadOnChange { get; set; } = true;

        #endregion
    }
}