using System.IO;

namespace Nameless.Localization.Json {
    public class LocalizationSettings {
        #region Public Properties

        /// <summary>
        /// Gets or sets the localization resource folder path. The folder path must be relative to the application.
        /// </summary>
        public string ResourceFolderPath { get; set; } = Path.Combine ("Resources", "l10n");
        /// <summary>
        /// Gets or sets the default culture name.
        /// </summary>
        public string DefaultCultureName { get; set; } = "en-US";
        /// <summary>
        /// Gets or sets whether will watch localization resource files for changes and reload.
        /// </summary>
        public bool Watch { get; set; } = true;

        #endregion
    }
}