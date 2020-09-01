using System.IO;

namespace Nameless.FileStorage.FileSystem {
    public sealed class FileSystemStorageSettings {

        #region Public Properties

        /// <summary>
        /// Gets or sets the root path of the file storage. Default value is
        /// the <see cref="FileSystemStorageSettings" /> assembly reside path, plus
        /// "App_Data" folder.
        /// </summary>
        public string Root { get; set; } = Path.Combine (typeof (FileSystemStorageSettings).Assembly.GetDirectoryPath (), "App_Data");

        #endregion
    }
}