namespace Nameless.FileProvider.Physical {
    public class FileProviderSettings {
        #region Public Properties

        public string Root { get; set; } = typeof (FileProviderSettings).Assembly.GetDirectoryPath ();

        #endregion
    }
}