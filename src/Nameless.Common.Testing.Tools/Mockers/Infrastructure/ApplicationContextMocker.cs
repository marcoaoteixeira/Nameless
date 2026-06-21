using Nameless.Application;
using Nameless.IO.FileSystem;

namespace Nameless.Testing.Tools.Mockers.Infrastructure;

public class ApplicationContextMocker : Mocker<IApplicationContext> {
    public ApplicationContextMocker WithEnvironment(string returnValue) {
        MockInstance.Setup(mock => mock.EnvironmentName)
                    .Returns(returnValue);

        return this;
    }

    public ApplicationContextMocker WithApplicationName(string returnValue) {
        MockInstance.Setup(mock => mock.ApplicationName)
                    .Returns(returnValue);

        return this;
    }

    public ApplicationContextMocker WithBaseDirectoryPath(string returnValue) {
        MockInstance.Setup(mock => mock.BaseDirectoryPath)
                    .Returns(returnValue);

        return this;
    }

    public ApplicationContextMocker WithFileSystemProvider(IFileSystemProvider returnValue) {
        MockInstance.Setup(mock => mock.FileSystemProvider)
                    .Returns(returnValue);

        return this;
    }

    public ApplicationContextMocker WithVersion(string returnValue) {
        MockInstance.Setup(mock => mock.Version)
                    .Returns(returnValue);

        return this;
    }
}