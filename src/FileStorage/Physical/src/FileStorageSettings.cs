using System.IO;

namespace Nameless.FileStorage.Physical {
    public sealed class FileStorageSettings {

        #region Public Properties

        /// <summary>
        /// Gets or sets the root path of the file storage. Default value is
        /// the <see cref="FileStorageSettings" /> assembly reside path, plus
        /// "App_Data" folder.
        /// </summary>
        public string Root { get; set; } = Path.Combine (typeof (FileStorageSettings).Assembly.GetDirectoryPath (), "App_Data");

        #endregion
    }
}