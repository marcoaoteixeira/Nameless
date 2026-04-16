using Moq;
using Nameless.IO.FileSystem;

namespace Nameless.Testing.Tools.Mockers.IO;

public class FileMocker : Mocker<IFile> {
    public FileMocker WithName(string returnValue) {
        MockInstance
            .Setup(mock => mock.Name)
            .Returns(returnValue);

        return this;
    }

    public FileMocker WithPath(string returnValue) {
        MockInstance
            .Setup(mock => mock.Path)
            .Returns(returnValue);

        return this;
    }

    public FileMocker WithExists(bool returnValue) {
        MockInstance
            .Setup(mock => mock.Exists)
            .Returns(returnValue);

        return this;
    }

    public FileMocker WithLastWriteTime(DateTime returnValue) {
        MockInstance
            .Setup(mock => mock.LastWriteTime)
            .Returns(returnValue);

        return this;
    }

    public FileMocker WithOpen(Stream returnValue) {
        MockInstance
            .Setup(mock => mock.Open(It.IsAny<FileMode>(), It.IsAny<FileAccess>(), It.IsAny<FileShare>()))
            .Returns(returnValue);

        return this;
    }

    public FileMocker ThrowOnOpen() {
        return ThrowOnOpen(() => new Exception(message: "Throw error on Open call."));
    }

    public FileMocker ThrowOnOpen<TException>(Func<TException> factory)
        where TException : Exception {
        MockInstance
            .Setup(mock => mock.Open(It.IsAny<FileMode>(), It.IsAny<FileAccess>(), It.IsAny<FileShare>()))
            .Throws(factory);

        return this;
    }

    public FileMocker ThrowOnDelete() {
        return ThrowOnDelete(() => new Exception(message: "Throw error on Delete call."));
    }

    public FileMocker ThrowOnDelete<TException>(Func<TException> factory)
        where TException : Exception {
        MockInstance
            .Setup(mock => mock.Delete())
            .Throws(factory);

        return this;
    }

    public FileMocker WithCopy(IFile returnValue) {
        MockInstance
            .Setup(mock => mock.Copy(It.IsAny<string>(), It.IsAny<bool>()))
            .Returns(returnValue);

        return this;
    }

    public FileMocker ThrowOnCopy() {
        return ThrowOnCopy(() => new Exception(message: "Throw error on Copy call."));
    }

    public FileMocker ThrowOnCopy<TException>(Func<TException> factory)
        where TException : Exception {
        MockInstance
            .Setup(mock => mock.Copy(It.IsAny<string>(), It.IsAny<bool>()))
            .Throws(factory);

        return this;
    }
}