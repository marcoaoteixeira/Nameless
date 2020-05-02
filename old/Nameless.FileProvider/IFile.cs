using System.IO;

namespace Nameless.FileProvider {
    public interface IFile {
        #region Properties

        bool Exists { get; }
        long Length { get; }
        string Path { get; }

        #endregion

        #region Methods

        Stream GetStream ();

        #endregion
    }
}