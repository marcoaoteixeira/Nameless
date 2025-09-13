using Moq;
using Nameless.IO.FileSystem;

namespace Nameless.Testing.Tools.Mockers.IO;

public class DirectoryMocker : Mocker<IDirectory> {
    public DirectoryMocker WithName(string returnValue) {
        MockInstance
            .Setup(mock => mock.Name)
            .Returns(returnValue);

        return this;
    }

    public DirectoryMocker WithPath(string returnValue) {
        MockInstance
            .Setup(mock => mock.Path)
            .Returns(returnValue);

        return this;
    }

    public DirectoryMocker WithExists(bool returnValue) {
        MockInstance
            .Setup(mock => mock.Exists)
            .Returns(returnValue);

        return this;
    }

    public DirectoryMocker ThrowOnCreate() {
        return ThrowOnCreate(() => new Exception("Throw error on Create call."));
    }

    public DirectoryMocker ThrowOnCreate<TException>(Func<TException> factory)
        where TException : Exception {
        MockInstance
            .Setup(mock => mock.Create())
            .Throws(factory);

        return this;
    }

    public DirectoryMocker WithGetFiles(IEnumerable<IFile> returnValue) {
        MockInstance
            .Setup(mock => mock.GetFiles(It.IsAny<string>(), It.IsAny<bool>()))
            .Returns(returnValue);

        return this;
    }

    public DirectoryMocker ThrowOnDelete() {
        return ThrowOnDelete(() => new Exception("Throw error on Delete call."));
    }

    public DirectoryMocker ThrowOnDelete<TException>(Func<TException> factory)
        where TException : Exception {
        MockInstance
            .Setup(mock => mock.Delete(It.IsAny<bool>()))
            .Throws(factory);

        return this;
    }
}
