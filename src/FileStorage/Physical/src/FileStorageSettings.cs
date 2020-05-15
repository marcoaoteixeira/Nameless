using System.IO;

namespace Nameless.FileStorage.Physical {
    public sealed class FileStorageSettings {

        #region Public Properties

        public string Root { get; set; } = Path.Combine (typeof (FileStorageSettings).Assembly.GetDirectoryPath (), "App_Data");

        #endregion
    }
}