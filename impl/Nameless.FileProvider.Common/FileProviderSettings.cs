namespace Nameless.FileProvider.Common {
    public sealed class FileProviderSettings {
        #region Public Properties

        /// <summary>
        /// Gets or sets the file provider root.
        /// </summary>
        public string Root { get; set; } = typeof (FileProviderSettings).Assembly.GetDirectoryPath ();

        #endregion
    }
}