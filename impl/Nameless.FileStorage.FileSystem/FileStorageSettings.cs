using System.IO;

namespace Nameless.FileStorage.FileSystem {
    public sealed class FileStorageSettings {

        #region Public Properties

        public string Root { get; set; } = Path.Combine (typeof (FileStorageSettings).Assembly.GetDirectoryPath (), "App_Data");

        #endregion
    }
}
