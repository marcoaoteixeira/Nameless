using Nameless.Infrastructure;

namespace Nameless.Testing.Tools.Mockers;

public sealed class ApplicationContextMocker : Mocker<IApplicationContext> {
    public ApplicationContextMocker WithEnvironment(string returnValue) {
        MockInstance.Setup(mock => mock.Environment)
                    .Returns(returnValue);

        return this;
    }

    public ApplicationContextMocker WithAppName(string returnValue) {
        MockInstance.Setup(mock => mock.AppName)
                    .Returns(returnValue);

        return this;
    }

    public ApplicationContextMocker WithAppBasePath(string returnValue) {
        MockInstance.Setup(mock => mock.AppFolderPath)
                    .Returns(returnValue);

        return this;
    }

    public ApplicationContextMocker WithAppDataFolderPath(string returnValue) {
        MockInstance.Setup(mock => mock.AppDataFolderPath)
                    .Returns(returnValue);

        return this;
    }

    public ApplicationContextMocker WithSemVer(string resul) {
        MockInstance.Setup(mock => mock.Version)
                    .Returns(resul);

        return this;
    }
}