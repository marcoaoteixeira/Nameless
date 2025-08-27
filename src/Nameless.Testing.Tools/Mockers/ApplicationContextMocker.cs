using Nameless.Infrastructure;

namespace Nameless.Testing.Tools.Mockers;

public sealed class ApplicationContextMocker : Mocker<IApplicationContext> {
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

    public ApplicationContextMocker WithDataDirectoryPath(string returnValue) {
        MockInstance.Setup(mock => mock.DataDirectoryPath)
                    .Returns(returnValue);

        return this;
    }

    public ApplicationContextMocker WithVersion(string returnValue) {
        MockInstance.Setup(mock => mock.Version)
                    .Returns(returnValue);

        return this;
    }
}