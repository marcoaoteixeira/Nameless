using Moq;
using Nameless.IO.FileSystem;

namespace Nameless.Testing.Tools.Mockers.IO;

public class FileSystemMocker : Mocker<IFileSystem> {
    public FileSystemMocker WithRoot(string returnValue) {
        MockInstance
            .Setup(mock => mock.Root)
            .Returns(returnValue);

        return this;
    }

    public FileSystemMocker WithGetDirectory(IDirectory returnValue) {
        MockInstance
            .Setup(mock => mock.GetDirectory(It.IsAny<string>()))
            .Returns(returnValue);

        return this;
    }

    public FileSystemMocker WithGetFile(IFile returnValue) {
        MockInstance
            .Setup(mock => mock.GetFile(It.IsAny<string>()))
            .Returns(returnValue);

        return this;
    }

    public FileSystemMocker WithGetFullPath(string returnValue) {
        MockInstance
            .Setup(mock => mock.GetFullPath(It.IsAny<string>()))
            .Returns(returnValue);

        return this;
    }
}
